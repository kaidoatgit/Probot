using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Probot.SubscriptionApi.Clients.IClients;
using Probot.SubscriptionApi.Config;
using Probot.Data;
using Probot.Data.Entities;
using Probot.SubscriptionApi.Services.Services.IServices;
using Probot.Shared.Enums;

namespace Probot.SubscriptionApi.Services.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly ServiceConfiguration _serviceConfiguration;
        private readonly ProbotContext _context;
        private readonly ICoingeckoClient _coingeckoClient;
        private readonly IMemoryCache _memoryCache;
        private const string _cacheKey = "solana_price";

        public TransactionService(IOptions<ServiceConfiguration> serviceConfiguration, ProbotContext context, ICoingeckoClient coingeckoClient, IMemoryCache memoryCache)
        {
            _context = context;
            _serviceConfiguration = serviceConfiguration.Value;
            _coingeckoClient = coingeckoClient;
            _memoryCache = memoryCache;
        }

        public async Task<Transaction> CreateTransaction(Order order, decimal totalPrice)
        {
            // if (!_memoryCache.TryGetValue(_cacheKey, out decimal currentSolanaUsdPrice))
            // {
            //     // Cache is empty or expired, fetch the price
            //     Solana? solanaPrice = (await _coingeckoClient.GetPriceTokenById("solana"))?.Solana;
            //     if (solanaPrice == null || solanaPrice.Usd <= 0)
            //     {
            //         throw new ServiceException(StatusCodes.Status500InternalServerError, "It was not possible to calculate the cryptocurrency price at this time.");
            //     }

            //     currentSolanaUsdPrice = solanaPrice.Usd;
            //     var cacheEntryOptions = new MemoryCacheEntryOptions()
            //         .SetAbsoluteExpiration(TimeSpan.FromSeconds(30));

            //     _memoryCache.Set(_cacheKey, currentSolanaUsdPrice, cacheEntryOptions);
            // }

            // decimal amountToPay = Math.Round(totalPrice / currentSolanaUsdPrice, 4, MidpointRounding.AwayFromZero);
            decimal amountToPay = 0.01m;
            Transaction transaction = new()
            {
                TotalAmount = amountToPay,
                Token = Token.SOL,
                OrderId = order.Id,
                PaymentAddress = order.User.WalletAddress,
                RecipientAddress = _serviceConfiguration.RecipientAddress
            };

            _context.Transactions.Add(transaction);
            await _context.SaveChangesAsync();
            return transaction;
        }
    }
}
