using Microsoft.Extensions.Options;
using Probot.SubscriptionApi.Clients.IClients;
using Probot.SubscriptionApi.Config;
using Probot.SubscriptionApi.Services.BackgroundServices.IServices;
using Probot.SubscriptionApi.Services.BackgroundServices.Models;
using System.Threading.Channels;

namespace Probot.SubscriptionApi.Services.BackgroundServices.Helpers
{
    public class MonitorService : IMonitorService, IDisposable
    {
        private readonly SemaphoreSlim _orderSignal;
        private CancellationTokenSource _cancellationTokenSource;
        private bool _disposed = false;
        private readonly Channel<TransactionMessage> _transactionChannel;
        private string? _latestHash;
        private readonly Task<string?> _startingHashTask;
        private readonly ISolanaRpcClient _rpcClient;
        private readonly ServiceConfiguration _serviceConfiguration;

        public MonitorService(ISolanaRpcClient rpcClient, IOptions<ServiceConfiguration> serviceConfiguration)
        {
            _orderSignal = new(0);
            _cancellationTokenSource = new();
            _transactionChannel = Channel.CreateUnbounded<TransactionMessage>();
            _rpcClient = rpcClient;
            _serviceConfiguration = serviceConfiguration.Value;
            //pensar em trocar para timer futuramente
            _startingHashTask = Task.Run(() => GetStartingHashAsync());
        }

        public async Task<string?> GetStartingHashAsync()
        {
            var clientResponse = await _rpcClient.GetTransactionsHashAsync(_serviceConfiguration.RecipientAddress, numberOfTransactions: 1);
            if (clientResponse.Data == null)
            {
                return null;
            }
            else
            {
                var hashs = clientResponse.Data;
                if (!hashs.Any())
                {
                    return string.Empty;
                }
                else
                {
                    return hashs.First();
                }
            }
        }

        public string? LatestHash
        {
            get
            {
                if (_startingHashTask != null && string.IsNullOrEmpty(_latestHash))
                {
                    _latestHash = _startingHashTask.Result;
                }
                return _latestHash;
            }
            set
            {
                _latestHash = value;
            }
        }
        

        public Channel<TransactionMessage> TransactionChannel
        {
            get
            {
                return _transactionChannel;
            }
        }

        public void CloseChannel()
        {
            _transactionChannel.Writer.TryComplete();
        }

        public CancellationToken Token
        {
            get
            {
                if (_cancellationTokenSource.IsCancellationRequested)
                {
                    _cancellationTokenSource.Dispose();
                    _cancellationTokenSource = new();
                }
                return _cancellationTokenSource.Token;
            }
        }


        public void Cancel()
        {
            _cancellationTokenSource.Cancel();
        }

        public async Task WaitForOrderSignalAsync(CancellationToken cancellationToken)
        {
            await _orderSignal.WaitAsync(cancellationToken);
        }

        public void NotifyNewOrder()
        {
            _orderSignal.Release(2);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    //Managed resource
                    _cancellationTokenSource.Dispose();
                    _orderSignal.Dispose();
                }
                //Unmanaged resource
                _disposed = true;
            }
        }
    }
}
