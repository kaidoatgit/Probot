using ProPayments.Service.Data.Entities;

namespace ProPayments.Service.Services.Services.IServices;

public interface IProductKeyService
{
    Task<IEnumerable<ProductKey>> GenerateProductKeys(Order order);
    Task<ProductKey> GetProductKeyAsync(string code, ulong userId, bool isActivated, bool includeReferences = false);
    Task<IEnumerable<ProductKey>> GetProductKeysAsync(ulong userId, bool isActivated);
}
