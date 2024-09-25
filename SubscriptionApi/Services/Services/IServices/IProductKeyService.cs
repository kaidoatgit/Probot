
using Probot.Data.Entities;

namespace Probot.SubscriptionApi.Services.Services.IServices;
public interface IProductKeyService
{
    Task<IEnumerable<ProductKey>> GenerateProductKeys(Order order, CancellationToken cancellationToken);
    Task<ProductKey> GetProductKeyByCodeAsync(string code, ulong? userId = null, bool? isActivated = null, bool includeReferences = false);
    Task<IEnumerable<ProductKey>> GetProductKeysAsync(ulong? userId = null, bool? isActivated = null);
    Task<Dictionary<ulong, int>> GetNonActivatedProductKeysPerUserAsync(CancellationToken cancellationToken);
}
