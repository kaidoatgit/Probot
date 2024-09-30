
using Probot.Data.Entities;

namespace Probot.SubscriptionApi.Services.Services.IServices;
public interface IProductKeyService
{
    Task<IEnumerable<ProductKey>> GenerateProductKeysAsync(Order order, CancellationToken cancellationToken);
    Task<IEnumerable<ProductKey>> GenerateProductKeysAsync(int amount, int period);
    Task<ProductKey> GetProductKeyByCodeAsync(string code, bool includeReferences = false, ulong? userId = null, bool? isActivated = null);
    Task<IEnumerable<ProductKey>> GetProductKeysAsync(ulong? userId = null, bool? isActivated = null);
    Task<ProductKey> ClaimProductKeyAsync(string code, ulong userId);
}
