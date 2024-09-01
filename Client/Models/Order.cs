using System.ComponentModel.DataAnnotations.Schema;
using DSharpPlus.Entities;
using Newtonsoft.Json;

namespace Probot.Client.Models
{
    public class Order
    {
        public ulong Id { get; set; }
        public User User { get; set; } = null!;
        public Invoice Invoice { get; set; } = null!;

        [NotMapped]
        public Interaction? Interaction { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this, Formatting.None);
        }
    }

    public class Interaction
    {
        public ulong MessageId { get; }
        public DiscordInteraction DiscordInteraction { get; }

        public Interaction(ulong messageId, DiscordInteraction discordInteraction)
        {
            MessageId = messageId;
            DiscordInteraction = discordInteraction;
        }
    }
}
