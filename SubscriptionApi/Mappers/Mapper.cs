using Probot.Data.Entities;
using Probot.Shared.Dtos.Order.Request;
using Probot.Shared.Dtos.Order.Response;
using Probot.Shared.Dtos.Product.Request;
using Probot.Shared.Dtos.Product.Response;
using Probot.Shared.Dtos.ProductKey.Response;
using Probot.Shared.Dtos.ProductOption.Response;
using Probot.Shared.Dtos.ProRaffle.Response;
using Probot.Shared.Dtos.Subscription.Response;
using Probot.Shared.Dtos.User.Request;
using Probot.Shared.Dtos.User.Response;
using Probot.Shared.Dtos.UserSetting.Response;
using Riok.Mapperly.Abstractions;

namespace Probot.SubscriptionApi.Mappers
{
    [Mapper]
    public partial class Mapper
    {
        
        [MapDerivedType(typeof(ProRaffle), typeof(ProRaffleResponse))]
        public partial UserSettingResponse MapToUserSettingResponse(UserSetting userSetting); 
        [MapNestedProperties(nameof(Subscription.ProductOption))]
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
