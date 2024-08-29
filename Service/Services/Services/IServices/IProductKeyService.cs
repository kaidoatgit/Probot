using ProPayments.Service.Data.Entities;

namespace ProPayments.Service.Services.Services.IServices;

public interface IProductKeyService
{
    Task<IEnumerable<ProductKey>> GenerateProductKeys(Order order, CancellationToken cancellationToken);
    Task<ProductKey> GetProductKeyByCodeAsync(string code, bool? isActivated=null, bool includeReferences = false);
    Task<IEnumerable<ProductKey>> GetProductKeysForUserAsync(ulong userId, bool isActivated);
    Task<Dictionary<ulong, int>> GetNonActivatedProductKeysPerUserAsync(CancellationToken cancellationToken);
}
