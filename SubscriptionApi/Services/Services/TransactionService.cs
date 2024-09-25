using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Probot.SubscriptionApi.Clients.IClients;
using Probot.SubscriptionApi.Options;
using Probot.Data;
using Probot.Data.Entities;
using Probot.SubscriptionApi.Services.Services.IServices;
using Probot.Shared.Enums;
using Probot.SubscriptionApi.Clients.Dtos.Coingecko.Response;
using Probot.SubscriptionApi.Exceptions;

namespace Probot.SubscriptionApi.Services.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly SubscriptionSettings _subscriptionSettings;
        private readonly ProbotContext _context;
        private readonly ICoingeckoClient _coingeckoClient;
        private readonly IMemoryCache _memoryCache;
        private const string _cacheKey = "solana_price";

        public TransactionService(IOptions<SubscriptionSettings> subscriptionOptions, ProbotContext context, ICoingeckoClient coingeckoClient, IMemoryCache memoryCache)
        {
            _context = context;
            _subscriptionSettings = subscriptionOptions.Value;
            _coingeckoClient = coingeckoClient;
            _memoryCache = memoryCache;
        }

        public async Task<Transaction> CreateTransaction(Order order, decimal totalPrice)
        {
            if (!_memoryCache.TryGetValue(_cacheKey, out decimal currentSolanaUsdPrice))
            {
                Solana? solanaPrice = (await _coingeckoClient.GetPriceTokenById("solana"))?.Solana;
                if (solanaPrice == null || solanaPrice.Usd <= 0)
                {
                    throw new SubscriptionException(ExceptionResult.InternalServerError500, "It was not possible to calculate the cryptocurrency price at this time.");
                }

                currentSolanaUsdPrice = solanaPrice.Usd;
                var cacheEntryOptions = new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromSeconds(30));
                _memoryCache.Set(_cacheKey, currentSolanaUsdPrice, cacheEntryOptions);
            }

            decimal amountToPay = Math.Round(totalPrice / currentSolanaUsdPrice, 4, MidpointRounding.AwayFromZero);
            Transaction transaction = new()
            {
                TotalAmount = amountToPay,
                Coin = Coin.SOL,
                OrderId = order.Id,
                PaymentAddress = order.User.WalletAddress,
                RecipientAddress = _subscriptionSettings.RecipientAddress
            };

            _context.Transactions.Add(transaction);
            await _context.SaveChangesAsync();
            return transaction;
        }
    }
}
