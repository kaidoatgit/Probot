using Newtonsoft.Json;
using System.Text;

namespace ProPayments.Client.Models
{
    public class User
    {
        public ulong Id { get; set; }
        public string Username { get; private set; }
        public string WalletAddress { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public List<Subscription> Subscriptions { get; set; } = new();

        public User(ulong id, string username, string walletAddress)
        {
            Id = id;
            Username = username;
            WalletAddress = walletAddress;
        }

        public User(User user)
        {
            Id = user.Id;
            Username = user.Username;
            WalletAddress = user.WalletAddress;
            Email = user.Email;
            Subscriptions = user.Subscriptions;
        }

        public override string ToString()
        {
            StringBuilder stringBuilder = new();
            stringBuilder.AppendLine($"Username: {Username}");
            foreach (var subscription in Subscriptions)
            {
                stringBuilder.AppendLine(subscription.ToString());
            }
            return stringBuilder.ToString();
        }

        public void RemoveSubscription(ulong subscriptionPlanRoleId)
        {
            if (Subscriptions != null)
            {
                Subscriptions.RemoveAll(s => s.PlanRoleId == subscriptionPlanRoleId);
            }
        }

        public void AddOrUpdateSubscription(Subscription subscription)
        {
            var existingSubscription = Subscriptions.FirstOrDefault(s => s.PlanRoleId == subscription.PlanRoleId);
            if (existingSubscription != null)
            {
                var index = Subscriptions.IndexOf(existingSubscription);
                if (index >= 0)
                {
                    Subscriptions[index] = subscription;
                }
            }
            else
            {
                Subscriptions?.Add(subscription);
            }
        }
    }
}
