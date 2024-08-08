//using Newtonsoft.Json;
//using ProPayments.Service.Clients.Dtos.Solana.Response;
//using System.Net.WebSockets;
//using System.Text;

namespace ProPayments.Service.Clients
{
    public class SolanaWebSocketClient
    {
        //private string _accountPubkey = "9cuAgoYBNDCiVBRV5XgiXgcfRuR8zukCnfJ6zwbQv6N8";
        //private ClientWebSocket _webSocket = new();
        //private string _webSocketUrl = "wss://api.mainnet-beta.solana.com";
        ////private string _webSocketUrl = "wss://api.testnet.solana.com";
        //private SolanaRpcClient _rpcClient;
        //private CancellationTokenSource _cancellationTokenSource; // não é usado atualmente
        //private Task? _backgroundTask;
        //private DateTime _lastFetchTime = DateTime.MinValue;
        //private TimeSpan _fetchInterval = TimeSpan.FromSeconds(10);

        //public SolanaWebSocketClient(SolanaRpcClient rpcClient)
        //{
        //    _rpcClient = rpcClient;
        //    _cancellationTokenSource = new CancellationTokenSource();
        //    Console.WriteLine("Solana websocket created");
        //}
        //public WebSocketState CurrentState
        //{
        //    get
        //    {
        //        return _webSocket.State;
        //    }
        //}

        //public async Task ConnectAndSubscribe()
        //{
        //    if (_webSocket.State == WebSocketState.Open)
        //        return;

        //    await _webSocket.ConnectAsync(new Uri(_webSocketUrl), CancellationToken.None);
        //    Console.WriteLine("Connected to Solana WebSocket");

        //    var message = new
        //    {
        //        jsonrpc = "2.0",
        //        id = 1,
        //        method = "accountSubscribe",
        //        @params = new object[]
        //        {
        //            _accountPubkey,
        //            new { encoding = "jsonParsed" }
        //        }
        //    };

        //    string messageJson = JsonConvert.SerializeObject(message);
        //    var bytesToSend = Encoding.UTF8.GetBytes(messageJson);
        //    var arraySegment = new ArraySegment<byte>(bytesToSend);

        //    await _webSocket.SendAsync(arraySegment, WebSocketMessageType.Text, true, CancellationToken.None);
        //    _backgroundTask = Task.Run(async () => await ReceiveMessages());
        //}

        //public async Task Close()
        //{
        //    if (_webSocket.State == WebSocketState.Open)
        //    {
        //        await _webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", CancellationToken.None);
        //        _cancellationTokenSource.Cancel();
        //        if (_backgroundTask != null)
        //        {
        //            await _backgroundTask;
        //        }
        //    }
        //}

        //private async Task ReceiveMessages()
        //{
        //    var buffer = new byte[1024 * 4];
        //    var previousBalance = await _rpcClient.GetBalanceAsync(_accountPubkey);
        //    var processedSignatures = await _rpcClient.GetTransactionsHashAsync(_accountPubkey);
        //    int counter = 0;

        //    try
        //    {
        //        await Task.Delay(TimeSpan.FromSeconds(150));
        //        while (_webSocket.State == WebSocketState.Open)
        //        {
        //            counter++;
        //            var result = await _webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
        //            if (result.MessageType == WebSocketMessageType.Text)
        //            {
        //                var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
        //                Console.WriteLine("Received data: " + message);

        //                var parsedData = JsonConvert.DeserializeObject<WebSocketResponse>(message) ?? new();
        //                string method = parsedData.Method;
        //                long currentBalance = parsedData.Params?.Result?.Value?.Lamports ?? 0;

        //                if (method == "accountNotification" && currentBalance > previousBalance)
        //                {
        //                    Console.WriteLine($"Entrei: {counter}");
        //                    List<string> currentSignatures = await _rpcClient.GetTransactionsHashAsync(_accountPubkey);
        //                    List<string> newSignatures = currentSignatures.Except(processedSignatures).ToList();
        //                    if (newSignatures.Any())
        //                    {
        //                        Console.WriteLine($"Novas assinaturas: {newSignatures.Count}");
        //                        //await _rpcClient.FetchAndProcessRecentTransactions(newSignatures);
        //                        processedSignatures = currentSignatures;
        //                    }
        //                    Console.WriteLine($"Sai: {counter}");
        //                }

        //                if (currentBalance > 0)
        //                {
        //                    previousBalance = currentBalance;
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        Console.WriteLine("WebSocket error: " + e.Message);
        //    }
        //    finally
        //    {
        //        if (_webSocket.State == WebSocketState.Open)
        //        {
        //            await _webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", CancellationToken.None);
        //        }
        //    }
        //}
    }
}
