using ProPayments.Client.Dtos.Plan.Response;
using ProPayments.Client.Dtos.Plan.Request;
using ProPayments.Client.Dtos.Subscription.Response;
using ProPayments.Client.Models;
using Riok.Mapperly.Abstractions;
using ProPayments.Client.Dtos.Order.Response;
using ProPayments.Client.Dtos.User.Response;

namespace ProPayments.Client.Mappers
{
    [Mapper]
    public partial class Mapper
    {
        public partial User MapToUser(UserWithSubscriptions apiResponse);
        public partial Plan MapToPlan(PlanWithOptionsResponse apiResponse);
        public partial IEnumerable<UpdatePlanRoleIdRequest> MapToUpdatePlansRoleIdRequest(List<Plan> plans);
        public partial Subscription MapToSubscription(SubscriptionResponse apiResponse);
        public partial Order MapToOrder(OrderResponse apiResponse);
        public partial User MapToUser(UserResponse apiResponse);









        //User mapper
        //public partial User MapToUser(GetUserResponse userDto);
        //public partial User MapToUser(UserResponse? apiResponse);
        //public partial Subscription MapToUser(GetSubscriptionResponse subscriptionDto);
        //public partial ManagerResponse<User> MapToUser(ApiResponse<UserResponse> apiResponse);


        ////Plan mapper
        //public partial Plan MapToPlan(GetPlanWithOptionsResponse planDto);
        //public partial IEnumerable<UpdatePlanRoleIdRequest> MapToUpdatePlanRoleIdRequestList(List<Plan> plans);


        ////Order mapper
        ////public partial ManagerResponse<Order> MapToOrder(ApiResponse<CreateOrderResponse> apiResponse);
        //public partial Order MapToOrder(OrderResponse? apiResponse);


        ////Subscription mapper
        ////public partial ManagerResponse<Subscription> MapToSubscription(ApiResponse<CreateSubscriptionResponse> apiResponse);
        //public partial Subscription MapToSubscription(SubscriptionResponse? apiResponse);


        ////Transaction mapper
        //public partial CompleteTransactionRequest MapToCompleteTransactionRequest(Transaction transaction);





        //[MapProperty(nameof(GetPlansResponse.Id), nameof(PlanOption.PlanId))]
        //public partial PlanOption MapToPlanDetails(GetPlansResponse planDto);

        //[MapNestedProperties(nameof(Invoice.Subscription))]
        //[MapNestedProperties($"{nameof(Invoice.Subscription)}.{nameof(Subscription.Plan)}")]
        //[MapNestedProperties($"{nameof(Invoice.Order)}.{nameof(Order.User)}")]
        //public partial CreateInvoiceDto Map(Invoice invoice);
    }
}
