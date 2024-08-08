using DSharpPlus.Entities;

namespace ProPayments.Client.Models
{
    public class Interaction
    {
        public ulong FollowUpMessageId { get; }
        public DiscordInteraction DiscordInteraction { get; }

        public Interaction(ulong followUpMessageId, DiscordInteraction discordInteraction)
        {
            FollowUpMessageId = followUpMessageId;
            DiscordInteraction = discordInteraction;
        }
    }
}
