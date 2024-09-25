using Probot.Client.Models;
using Probot.Shared.Dtos.Order.Response;
using Probot.Shared.Dtos.Product.Request;
using Probot.Shared.Dtos.Product.Response;
using Probot.Shared.Dtos.ProductKey.Response;
using Probot.Shared.Dtos.ProRaffleSetting.Response;
using Probot.Shared.Dtos.Subscription.Response;
using Probot.Shared.Dtos.User.Response;
using Probot.Shared.Dtos.ProductSetting.Response;
using Riok.Mapperly.Abstractions;

namespace Probot.Client.Mappers
{
    [Mapper]
    public partial class Mapper
    {
        [MapProperty(nameof(UserMetricsResponse.ActiveSubsPerProduct), nameof(@User.Metrics.ActiveSubsPerProduct))]
        [MapProperty(nameof(UserMetricsResponse.InactiveKeysPerProduct), nameof(@User.Metrics.InactiveKeysPerProduct))]
        public partial User MapToUser(UserMetricsResponse apiResponse);
        public partial Product MapToProduct(ProductResponse apiResponse);
        public partial IEnumerable<UpdateProductRoleIdRequest> MapToUpdateProductsRoleIdRequest(List<Product> products);
        public partial Subscription MapToSubscription(SubscriptionResponse apiResponse);
        public partial Order MapToOrder(OrderResponse apiResponse);
        public partial User MapToUser(UserResponse apiResponse);
        public partial ProductKey MapToProductKey(ProductKeyResponse apiResponse);
        
        [MapDerivedType(typeof(ProRaffleSettingResponse), typeof(ProRaffleSetting))]
        public partial ProductSetting MapToProductSetting(ProductSettingResponse apiResponse);
        public partial ProRaffleSetting MapToProRaffleSetting(ProRaffleSettingResponse apiResponse);
    }
}
