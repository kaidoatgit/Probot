using Microsoft.Extensions.Options;
using Probot.SubscriptionApi.Clients.Dtos.Solana.Response;
using Probot.SubscriptionApi.Clients.IClients;
using Probot.SubscriptionApi.Options;
using Probot.SubscriptionApi.Services.BackgroundServices.IServices;
using System.Collections.Concurrent;

namespace Probot.SubscriptionApi.Services.BackgroundServices
{
    public class TransactionMonitorService : BackgroundService
    {
        private readonly ISolanaRpcClient _rpcClient;
        private readonly IMonitorService _monitorService;
        private static readonly TimeSpan _monitoringPeriod = TimeSpan.FromSeconds(10);
        private readonly SubscriptionSettings _serviceConfiguration;
        private readonly ConcurrentDictionary<string, int> _failedTransactions = new();
        private readonly SemaphoreSlim _processTransactionsSemaphore = new(1,1);

        public TransactionMonitorService(ISolanaRpcClient rpcClient, IMonitorService monitorService, IOptions<SubscriptionSettings> serviceConfiguration)
        {
            _rpcClient = rpcClient;
            _monitorService = monitorService;
            _serviceConfiguration = serviceConfiguration.Value;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    Console.WriteLine("[Transaction Monitor] -> Waiting new orders");
                    await _monitorService.WaitForOrderSignalAsync(stoppingToken);
                    await MonitorTransactions(stoppingToken);
                }
                catch (Exception e)
                {
                    Console.WriteLine($"[Transaction Monitor] -> Program shutdown | Reason: {e.Message}");
                }
            }
        }

        private async Task MonitorTransactions(CancellationToken externalToken)
        {
            var internalToken = _monitorService.Token;
            using CancellationTokenSource linkedCts = CancellationTokenSource.CreateLinkedTokenSource(externalToken, internalToken);
            var token = linkedCts.Token;

            try
            {
                string latestProcessedHash = _monitorService.LatestHash!;
                using var timer = new PeriodicTimer(_monitoringPeriod);
                while (!token.IsCancellationRequested && await timer.WaitForNextTickAsync(token))
                {
                    var (newTransactions, allTransactions) = await GetTransactions(latestProcessedHash, token);
                    if (allTransactions.Any())
                    {
                        Console.WriteLine($"Signatures detected: {allTransactions.Count}");
                        _ = Task.Run(async () => await ProcessTransactions(allTransactions, token));

                        if (newTransactions.Any())
                        {
                            // Update the latest processed signature after processing new transactions
                            latestProcessedHash = newTransactions.Last();
                            _monitorService.LatestHash = latestProcessedHash;
                        }
                    }
                    string message = latestProcessedHash == "" ? "No initial signature found" : $"Latest signature:{latestProcessedHash}";
                    Console.WriteLine(message);
                }
            }
            catch (OperationCanceledException)
            {
                if (internalToken.IsCancellationRequested)
                {
                    Console.WriteLine("[Transaction Monitor] -> Set to idle");
                }
                else if (externalToken.IsCancellationRequested)
                {
                    externalToken.ThrowIfCancellationRequested();
                }
            }
        }

        private async Task ProcessTransactions(List<string> transactionsHash, CancellationToken token)
        {
            await _processTransactionsSemaphore.WaitAsync(token);
            try
            {
                foreach (var hash in transactionsHash)
                {
                    try
                    {
                        var transactionDetail = await _rpcClient.GetTransactionDetailsAsync(hash);
                        if (transactionDetail != null)
                        {
                            var (txAddress, txAmount) = ExtractData(transactionDetail);
                            await _monitorService.TransactionChannel.Writer.WriteAsync(new(hash, txAddress, txAmount), token);
                            _failedTransactions.TryRemove(hash, out _);
                        }
                        else
                        {
                            HandleFailedTransaction(hash);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"[ProcessTransactions] - hash:{hash}, message: {ex.Message}");
                    }
                    finally
                    {
                        //If cancelation was requested before, when reaching this delay it will throw instantly
                        await Task.Delay(TimeSpan.FromSeconds(3), token);
                    }
                }
            }
            finally
            {
                _processTransactionsSemaphore.Release();
            }
        }

        private void HandleFailedTransaction(string hash)
        {
            if (_failedTransactions.TryGetValue(hash, out int failCount))
            {
                failCount++;
                if (failCount > 3)
                {
                    _failedTransactions.TryRemove(hash, out _);
                }
                else
                {
                    _failedTransactions[hash] = failCount;
                    Console.WriteLine($"> {_failedTransactions[hash]}x transaction({hash}) failed to process");
                }
            }
            else
            {
                _failedTransactions[hash] = 1;
                Console.WriteLine($"> {_failedTransactions[hash]}x transaction({hash}) failed to process");
            }

        }

        /// <summary>
        /// If the latest processed hash isnt available it will keep fetching until is found.
        /// If the recipient wallet is new there wont be any transaction hash yet, the process wont proceed until a base hash reference is set.
        /// </summary>
        /// <param name="latestProcessedHash"></param>
        /// <param name="token"></param>
        /// <returns>Retrieves transactions after the latest processed transaction hash</returns>
        private async Task<(List<string> newTransactions, List<string> transactions)> GetTransactions(string latestProcessedHash, CancellationToken token)
        {
            if (string.IsNullOrEmpty(latestProcessedHash))
            {
                string? startingHash;
                using var timer = new PeriodicTimer(TimeSpan.FromSeconds(10));
                do
                {
                    Console.WriteLine("Trying to get the latest hash...");
                    startingHash = await _monitorService.GetStartingHashAsync();
                } while (string.IsNullOrEmpty(startingHash) && await timer.WaitForNextTickAsync(token));

                latestProcessedHash = startingHash!;
                return (new() { latestProcessedHash }, new() { latestProcessedHash });
            }

            var clientResponse = await _rpcClient.GetTransactionsHashAsync(_serviceConfiguration.RecipientAddress, until: latestProcessedHash);
            clientResponse.Data ??= new List<string>();

            var newTransactions = clientResponse.Data;
            newTransactions.Reverse();

            var transactions = new List<string>(newTransactions);
            transactions.AddRange(_failedTransactions.Keys);

            return (newTransactions, transactions);
        }

        private (string, decimal) ExtractData(Result transactionDetails)
        {
            var data = transactionDetails.Transaction?.Message?.Instructions?.FirstOrDefault(i => i.Parsed != null)?.Parsed;
            if (data != null)
            {
                if (string.Equals(data.Type, "transfer", StringComparison.InvariantCultureIgnoreCase))
                {
                    var sourceAddress = data.Info!.Source;
                    var amount = Math.Round(data.Info!.Lamports / 1_000_000_000m, 4, MidpointRounding.AwayFromZero);
                    return (sourceAddress, amount);
                }
            }
            return (string.Empty, 0m);
        }

        public override void Dispose()
        {
            base.Dispose();
            _monitorService.CloseChannel();
            (_monitorService as IDisposable)?.Dispose();
        }
    }
}
