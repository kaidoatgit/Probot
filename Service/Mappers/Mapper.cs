using ProPayments.Service.Data.Entities;
using ProPayments.Service.Dtos.Orders.Request;
using ProPayments.Service.Dtos.Orders.Response;
using ProPayments.Service.Dtos.Plans.Request;
using ProPayments.Service.Dtos.Plans.Response;
using ProPayments.Service.Dtos.Subscriptions.Response;
using ProPayments.Service.Dtos.Users.Request;
using ProPayments.Service.Dtos.Users.Response;
using Riok.Mapperly.Abstractions;
using UserResponse = ProPayments.Service.Dtos.Users.Response.UserResponse;

namespace ProPayments.Service.Mappers
{
    [Mapper]
    public partial class Mapper
    {
        public partial UserWithSubscriptionsResponse MapToUserWithSubscriptionsResponse(User user);
        public partial PlanOptionResponse MapToPlanOptionResponse(PlanOption planOption);
        public partial OrderResponse MapToOrderResponse(Order order);
        public partial UserResponse MapToUserResponse(User user);

        [MapProperty($"{nameof(Subscription.Plan)}.{nameof(Plan.RoleId)}", $"{nameof(SubscriptionResponse.PlanRoleId)}")]
        public partial SubscriptionResponse MapToSubscriptionResponse(Subscription subscription);
        public partial PlanResponse MapToPlanResponse(Plan plan);
        public partial User MapToUserEntity(UserRequest request);
        public partial Plan MapToPlanEntity(UpdatePlanRoleIdRequest request);
        public partial Order MapToOrderEntity(OrderRequest request);
        public partial GetOrderResponse MapToGetOrderResponse(Order request);


        //User mapper
        //public partial UserResponse MapToCreateUserResponse(User user);
        //[MapProperty($"{nameof(Subscription.Plan)}.{nameof(Plan.RoleId)}", $"{nameof(GetSubscriptionResponse.PlanRoleId)}")]
        //public partial GetSubscriptionResponse MapToGetSubscriptionResponse(Subscription subscription);


        //[MapProperty(nameof(UpdatePlanRoleIdRequest.PlanType), nameof(Plan.Type))]
        //[MapProperty(nameof(UpdatePlanRoleIdRequest.PlanName), nameof(PlanType.Name))]
        //private partial PlanType MapToPlanType(UpdatePlanRoleIdRequest request);
        //public partial IEnumerable<Plan> MapToUpdatePlanRoleIdRequestList(IEnumerable<UpdatePlanRoleIdRequest> request);

        //Order mapper
        //[MapProperty(nameof(CreateOrderRequest.DiscordInteractionId), nameof(Order.DiscordInteractionId))]
        //private string MapDiscordInteractionId(ulong value) => value.ToString();

    }
}
