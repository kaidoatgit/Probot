using ProPayments.Client.Dtos.Subscription.Response;

namespace ProPayments.Client.Dtos.User.Response
{
    public class UsersWithSubscriptionsResponse
    {
        public List<UserWithSubscriptions> Users { get; set; } = new();
    }
}
