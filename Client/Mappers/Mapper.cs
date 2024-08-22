using ProPayments.Client.Dtos.Product.Response;
using ProPayments.Client.Dtos.Product.Request;
using ProPayments.Client.Dtos.Subscription.Response;
using ProPayments.Client.Models;
using Riok.Mapperly.Abstractions;
using ProPayments.Client.Dtos.Order.Response;
using ProPayments.Client.Dtos.User.Response;
using ProPayments.Client.Dtos.ProductKey.Response;
using ProPayments.Client.Dtos.ProRaffle.Response;

namespace ProPayments.Client.Mappers
{
    [Mapper]
    public partial class Mapper
    {
        public partial User MapToUser(UserWithSubscriptions apiResponse);
        public partial Product MapToProduct(ProductWithOptionsResponse apiResponse);
        public partial IEnumerable<UpdateProductRoleIdRequest> MapToUpdateProductsRoleIdRequest(List<Product> products);
        public partial Subscription MapToSubscription(SubscriptionResponse apiResponse);
        public partial Order MapToOrder(OrderResponse apiResponse);
        public partial User MapToUser(UserResponse apiResponse);
        public partial ProductKey MapToProductKey(ProductKeyResponse apiResponse);
        public partial ProRaffle MapToProRaffle(ProRaffleResponse apiResponse);









        //User mapper
        //public partial User MapToUser(GetUserResponse userDto);
        //public partial User MapToUser(UserResponse? apiResponse);
        //public partial Subscription MapToUser(GetSubscriptionResponse subscriptionDto);
        //public partial ManagerResponse<User> MapToUser(ApiResponse<UserResponse> apiResponse);


        ////Product mapper
        //public partial Product MapToProduct(GetProductWithOptionsResponse productDto);
        //public partial IEnumerable<UpdateProductRoleIdRequest> MapToUpdateProductRoleIdRequestList(List<Product> products);


        ////Order mapper
        ////public partial ManagerResponse<Order> MapToOrder(ApiResponse<CreateOrderResponse> apiResponse);
        //public partial Order MapToOrder(OrderResponse? apiResponse);


        ////Subscription mapper
        ////public partial ManagerResponse<Subscription> MapToSubscription(ApiResponse<CreateSubscriptionResponse> apiResponse);
        //public partial Subscription MapToSubscription(SubscriptionResponse? apiResponse);


        ////Transaction mapper
        //public partial CompleteTransactionRequest MapToCompleteTransactionRequest(Transaction transaction);





        //[MapProperty(nameof(GetProductsResponse.Id), nameof(ProductOption.ProductId))]
        //public partial ProductOption MapToProductDetails(GetProductsResponse productDto);

        //[MapNestedProperties(nameof(Invoice.Subscription))]
        //[MapNestedProperties($"{nameof(Invoice.Subscription)}.{nameof(Subscription.Product)}")]
        //[MapNestedProperties($"{nameof(Invoice.Order)}.{nameof(Order.User)}")]
        //public partial CreateInvoiceDto Map(Invoice invoice);
    }
}
