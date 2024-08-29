using ProPayments.Service.Data.Entities;
using ProPayments.Service.Dtos.ProductKeys.Response;
using ProPayments.Service.Dtos.Orders.Request;
using ProPayments.Service.Dtos.Orders.Response;
using ProPayments.Service.Dtos.Products.Request;
using ProPayments.Service.Dtos.Products.Response;
using ProPayments.Service.Dtos.Subscriptions.Response;
using ProPayments.Service.Dtos.Users.Request;
using ProPayments.Service.Dtos.Users.Response;
using Riok.Mapperly.Abstractions;
using UserResponse = ProPayments.Service.Dtos.Users.Response.UserResponse;
using ProPayments.Service.Dtos.UserSettings.Response;
using ProPayments.Service.Dtos.ProRaffles.Response;
using ProPayments.Service.Dtos.ProductOptions.Response;

namespace ProPayments.Service.Mappers
{
    [Mapper]
    public partial class Mapper
    {
        
        [MapDerivedType(typeof(ProRaffle), typeof(ProRaffleResponse))]
        public partial UserSettingResponse MapToUserSettingResponse(UserSetting userSetting); 
        [MapNestedProperties(nameof(Subscription.ProductOption))]
        public partial SubscriptionResponse MapToSubscriptionResponse(Subscription subscription);
        // public partial UserWithSubscriptionsResponse MapToUserWithSubscriptionsResponse(User user);
        [MapNestedProperties(nameof(User.Summary))]
        public partial UserSummaryResponse MapToUserSummaryResponse(User user);
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
