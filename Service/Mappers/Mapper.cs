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
using ProPayments.Service.Controllers;

namespace ProPayments.Service.Mappers
{
    [Mapper]
    public partial class Mapper
    {
        public partial UserWithSubscriptionsResponse MapToUserWithSubscriptionsResponse(User user);
        public partial ProductOptionResponse MapToProductOptionResponse(ProductOption productOption);
        public partial OrderResponse MapToOrderResponse(Order order);
        public partial UserResponse MapToUserResponse(User user);
        public partial SubscriptionResponse MapToSubscriptionResponse(Subscription subscription);

        [MapDerivedType(typeof(ProRaffle), typeof(ProRaffleResponse))]
        public partial UserSettingResponse MapToUserSettingResponse(UserSetting userSetting);  
        
        public partial ProductResponse MapToProductResponse(Product product);
        public partial User MapToUserEntity(UserRequest request);
        public partial Product MapToProductEntity(UpdateProductRoleIdRequest request);
        public partial Order MapToOrderEntity(OrderRequest request);
        public partial GetOrderResponse MapToGetOrderResponse(Order request);
        public partial ProductKeyResponse MapToProductKeyResponse(ProductKey productKey);

        [MapProperty($"{nameof(Subscription.Code)}", $"{nameof(ProRaffleSubscriptionResponse.Subscription)}.{nameof(Subscription.Code)}")]
        [MapProperty($"{nameof(Subscription.StartDate)}", $"{nameof(ProRaffleSubscriptionResponse.Subscription)}.{nameof(Subscription.StartDate)}")]
        [MapProperty($"{nameof(Subscription.EndDate)}", $"{nameof(ProRaffleSubscriptionResponse.Subscription)}.{nameof(Subscription.EndDate)}")]
        [MapProperty(nameof(Subscription.UserSetting), nameof(ProRaffleSubscriptionResponse.ProRaffle))]
        public partial ProRaffleSubscriptionResponse MapToProRaffleSubscriptionResponse(Subscription subscription);


        //User mapper
        //public partial UserResponse MapToCreateUserResponse(User user);
        //[MapProperty($"{nameof(Subscription.Product)}.{nameof(Product.RoleId)}", $"{nameof(GetSubscriptionResponse.ProductRoleId)}")]
        //public partial GetSubscriptionResponse MapToGetSubscriptionResponse(Subscription subscription);


        //[MapProperty(nameof(UpdateProductRoleIdRequest.Name), nameof(Product.Type))]
        //[MapProperty(nameof(UpdateProductRoleIdRequest.Name), nameof(ProductName.Name))]
        //private partial ProductName MapToProductName(UpdateProductRoleIdRequest request);
        //public partial IEnumerable<Product> MapToUpdateProductRoleIdRequestList(IEnumerable<UpdateProductRoleIdRequest> request);

        //Order mapper
        //[MapProperty(nameof(CreateOrderRequest.DiscordInteractionId), nameof(Order.DiscordInteractionId))]
        //private string MapDiscordInteractionId(ulong value) => value.ToString();

    }
}
