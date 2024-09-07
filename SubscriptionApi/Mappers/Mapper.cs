using Probot.Data.Entities;
using Probot.Shared.Dtos.Order.Request;
using Probot.Shared.Dtos.Order.Response;
using Probot.Shared.Dtos.Product.Request;
using Probot.Shared.Dtos.Product.Response;
using Probot.Shared.Dtos.ProductKey.Response;
using Probot.Shared.Dtos.ProductOption.Response;
using Probot.Shared.Dtos.ProRaffleSetting.Response;
using Probot.Shared.Dtos.Subscription.Response;
using Probot.Shared.Dtos.User.Request;
using Probot.Shared.Dtos.User.Response;
using Probot.Shared.Dtos.ProductSetting.Response;
using Riok.Mapperly.Abstractions;

namespace Probot.SubscriptionApi.Mappers
{
    [Mapper]
    public partial class Mapper
    {
        
        [MapDerivedType(typeof(ProRaffleSetting), typeof(ProRaffleSettingResponse))]
        public partial ProductSettingResponse MapToProductSettingResponse(ProductSetting productSetting); 
        [MapProperty(nameof(@Subscription.Product.RoleId), nameof(SubscriptionResponse.ProductRoleId))]
        [MapProperty(nameof(@Subscription.ProductKey.ProductOption.PeriodDescription), nameof(SubscriptionResponse.PeriodDescription))]
        public partial SubscriptionResponse MapToSubscriptionResponse(Subscription subscription);
        [MapNestedProperties(nameof(User.Metrics))]
        public partial UserMetricsResponse MapToUserMetricsResponse(User user);
        public partial ProductOptionResponse MapToProductOptionResponse(ProductOption productOption);
        public partial OrderResponse MapToOrderResponse(Order order);
        public partial UserResponse MapToUserResponse(User user);
 
        
        public partial ProductResponse MapToProductResponse(Product product);
        public partial User MapToUserEntity(UserRequest request);
        public partial Product MapToProductEntity(UpdateProductRoleIdRequest request);
        public partial Order MapToOrderEntity(OrderRequest request);
        public partial GetOrderResponse MapToGetOrderResponse(Order request);
        public partial ProductKeyResponse MapToProductKeyResponse(ProductKey productKey);
    }
}
