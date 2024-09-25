using DSharpPlus.Entities;
using Probot.Client.Clients.SubscriptionApi;
using Probot.Client.Mappers;
using Probot.Client.Models;

namespace Probot.Client.Managers
{
    public class ProductManager
    {
        private readonly List<Product> _products = new();
        private readonly ProductClient _productClient;
        private readonly Mapper _mapper;

        public ProductManager(ProductClient productClient, Mapper mapper)
        {
            _productClient = productClient;
            _mapper = mapper;
            Console.WriteLine("Product Manager created");
        }

        public List<Product> Products => _products;

        public async Task<bool> LoadProductsAsync(List<DiscordRole> guildRoles)
        {
            _products.Clear();
            var apiResponse = await _productClient.GetProductsWithOptionsAsync();
            if(apiResponse.Data == null || !apiResponse.Data.Any())
            {
                return false;
            }

            var products = apiResponse.Data.Select(p => _mapper.MapToProduct(p));
            var productsToUpdate = new List<Product>();
            foreach (var role in guildRoles)
            {
                var matchedProduct = products.FirstOrDefault(p => p.Name.ToString() == role.Name);
                if (matchedProduct != null)
                {
                    if(matchedProduct.RoleId != role.Id)
                    {
                        matchedProduct.RoleId = role.Id;
                        productsToUpdate.Add(matchedProduct);
                    }
                    _products.Add(matchedProduct);
                }
            }

            if(!_products.Any())
            {
                return false;
            }
            
            return await UpdateProductsRoleIdAsync(productsToUpdate);
        }

        private async Task<bool> UpdateProductsRoleIdAsync(List<Product> productsToUpdate)
        {
            if (!productsToUpdate.Any())
            {
                return true;
            }
            var productsRoleIdRequest = _mapper.MapToUpdateProductsRoleIdRequest(productsToUpdate);
            return await _productClient.UpdateProductsRoleIdAsync(productsRoleIdRequest);
        }

        public Product? GetProductByRoleId(string roleId)
        {
            Product? product = _products.FirstOrDefault(p => p.RoleId.ToString()!.Equals(roleId));
            return product;
        }

        public List<ProductOption>? GetDurationsWithPrices(string roleId)
        {
            Product? product = _products.FirstOrDefault(p => p.RoleId.ToString()!.Equals(roleId));
            return product?.ProductOptions;
        }

    }
}
