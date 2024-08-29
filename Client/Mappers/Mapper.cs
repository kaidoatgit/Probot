using ProPayments.Client.Dtos.Product.Response;
using ProPayments.Client.Dtos.Product.Request;
using ProPayments.Client.Dtos.Subscription.Response;
using ProPayments.Client.Models;
using Riok.Mapperly.Abstractions;
using ProPayments.Client.Dtos.Order.Response;
using ProPayments.Client.Dtos.User.Response;
using ProPayments.Client.Dtos.ProductKey.Response;
using ProPayments.Client.Dtos.UserSetting.Response;
using ProPayments.Client.Dtos.ProRaffle.Response;

namespace ProPayments.Client.Mappers
{
    [Mapper]
    public partial class Mapper
    {
        public partial User MapToUser(UserSummaryResponse apiResponse);
        public partial Product MapToProduct(ProductResponse apiResponse);
        public partial IEnumerable<UpdateProductRoleIdRequest> MapToUpdateProductsRoleIdRequest(List<Product> products);
        public partial Subscription MapToSubscription(SubscriptionResponse apiResponse);
        public partial Order MapToOrder(OrderResponse apiResponse);
        public partial User MapToUser(UserResponse apiResponse);
        public partial ProductKey MapToProductKey(ProductKeyResponse apiResponse);
        
        [MapDerivedType(typeof(ProRaffleResponse), typeof(ProRaffle))]
        public partial UserSetting MapToProRaffle(UserSettingResponse apiResponse);
    }
}
