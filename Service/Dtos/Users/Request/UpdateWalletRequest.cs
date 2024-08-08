using System.ComponentModel.DataAnnotations;

namespace ProPayments.Service.Dtos.Users.Request
{
    public class UpdateWalletRequest
    {
        [Required]
        public string WalletAddress { get; set; }
    }
}
