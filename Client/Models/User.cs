using System.Text;

namespace Probot.Client.Models
{
    public class User
    {
        public ulong Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string WalletAddress { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;        
        public Metrics? Metrics { get; set; }

        public User() {}
        
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
        }

        public override string ToString()
        {
            StringBuilder stringBuilder = new();
            stringBuilder.AppendLine($"{Username}|{Id}");

            if(Metrics?.ActiveSubsPerProduct.Count > 0)
            {
                stringBuilder.Append("-> Active subscription per product: ");
                foreach (var (product, activeSubsCount) in Metrics.ActiveSubsPerProduct)
                {
                    stringBuilder.Append($"({product}:{activeSubsCount})\t");
                }
            }
            stringBuilder.AppendLine(); 
            if(Metrics?.InactiveKeysPerProduct.Count > 0)
            {
                stringBuilder.Append("-> Inactivated keys per product: ");
                foreach (var (product, inactiveKeysCount) in Metrics.InactiveKeysPerProduct)
                {
                    stringBuilder.Append($"({product}:{inactiveKeysCount})");
                }
            }
            return stringBuilder.ToString();
        }
    }
    

    public class Metrics
    {
        public Dictionary<ulong, int> InactiveKeysPerProduct { get; set; } = new();
        public Dictionary<ulong, int> ActiveSubsPerProduct { get; set; } = new();
    }
}
