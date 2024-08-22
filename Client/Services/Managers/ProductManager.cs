using DSharpPlus.Entities;
using ProPayments.Client.Clients.ProPayments;
using ProPayments.Client.Mappers;
using ProPayments.Client.Models;

namespace ProPayments.Client.Services.Managers
{
    public class ProductManager
    {
        public List<Product> _products = new();
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
            if(apiResponse.Data == null)
            {
                return false;
            }

            //for the moment is a must to have products, but maybe in future could be an options for admins to add products
            if (!apiResponse.Data.Any())
            {
                return false;
            }

            var products = apiResponse.Data.Select(p => _mapper.MapToProduct(p)).ToList();
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

        private async Task<bool> UpdateProductsRoleIdAsync(List<Product> products)
        {
            if (!products.Any())
            {
                return true;
            }
            var productsRoleIdRequest = _mapper.MapToUpdateProductsRoleIdRequest(products);
            return await _productClient.UpdateProductsRoleIdAsync(productsRoleIdRequest);
        }

        public Product? GetProductByRoleId(string roleId)
        {
            Product? product = Products.FirstOrDefault(p => p.RoleId.ToString()!.Equals(roleId));
            return product;
        }

        public List<ProductOption>? GetDurationsWithPrices(string roleId)
        {
            Product? product = Products.FirstOrDefault(p => p.RoleId.ToString()!.Equals(roleId));
            return product?.ProductOptions;
        }

    }
}
