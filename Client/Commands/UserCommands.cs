using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using ProPayments.Client.Clients.ProPayments;
using ProPayments.Client.Services.Managers;

namespace ProPayments.Client.Commands
{
    public class UserCommands : ApplicationCommandModule
    {
        private readonly UserClient _userClient;
        private readonly PlanManager _planManager;
        public UserCommands(UserClient userClient, PlanManager planManager)
        {
            _userClient = userClient;
            _planManager = planManager;
        }

        [SlashCommand("add-apikey", "Register or update AlphaBot API Key to enable raffle automation")]
        public async Task AddApiCommand(InteractionContext ctx, [Option("key", "The API key provided by AlphaBot")] string key)
        {
           try
           {
               await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, 
                                new DiscordInteractionResponseBuilder().WithContent($"{key} registered successfully."));
           }
           catch (Exception ex)
           {
               Console.WriteLine(ex.Message);
           }
        }

        [SlashCommand("status", "Displays your current API key and indicates whether the raffle automation bot is running")]
        public async Task StatusCommand(InteractionContext ctx)
        {
           try
           {
               await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, 
                                new DiscordInteractionResponseBuilder().WithContent($"Bot is running."));
           }
           catch (Exception ex)
           {
               Console.WriteLine(ex.Message);
           }
        }
    }
}
