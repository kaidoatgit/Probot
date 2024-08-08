using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using Microsoft.Extensions.Configuration.UserSecrets;
using ProPayments.Client.Clients.ProPayments;
using ProPayments.Client.Helpers;
using ProPayments.Client.Services.Managers;
using System.Diagnostics.Metrics;
using System.Text;

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

        //[SlashCommand("register", "Register a new user with the provided API key")]
        //public async Task RegisterUserCommand(InteractionContext ctx, [Option("key", "The API key provided by AlphaBot for user registration")] string key)
        //{
        //    try
        //    {
        //        // Acknowledge the interaction immediately
        //        await ctx.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource);

        //        await Task.Delay(TimeSpan.FromMinutes(0.3));
        //        var channel = ctx.Channel;
        //        var message = await channel.GetMessageAsync(messageId);

        //        var newEmbed = new DiscordEmbedBuilder
        //        {
        //            Title = "Updated Title",
        //            Description = "This is the updated description.",
        //            Color = DiscordColor.Blurple
        //        };

        //        var messageBuilder = new DiscordMessageBuilder()
        //            .WithEmbed(newEmbed.Build());

        //        await message.ModifyAsync(messageBuilder);
        //        await ctx.FollowUpAsync(new DiscordFollowupMessageBuilder().WithContent("Message updated successfully.").AsEphemeral(true));
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine(ex.Message);
        //    }

        //    //// Create the initial buttons
        //    //var button1 = new DiscordButtonComponent(ButtonStyle.Primary, "button_1", "Button 1");
        //    //var button2 = new DiscordButtonComponent(ButtonStyle.Primary, "button_2", "Button 2");

        //    //// Create a message with the buttons
        //    //var message = new DiscordMessageBuilder()
        //    //    .WithContent("Press a button!")
        //    //    .AddComponents(button1, button2);

        //    //if (messageId == null)
        //    //{
        //    //    // Send the message and save its ID
        //    //    await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder(message));
        //    //    var sentMessage = await ctx.GetOriginalResponseAsync();
        //    //    messageId = sentMessage.Id;
        //    //}
        //    //else
        //    //{

        //    //    Console.WriteLine("entrei");
        //    //    var channel = ctx.Channel;
        //    //    var existingMessage = await channel.GetMessageAsync(messageId.Value);
        //    //    //await ctx.Channel.DeleteMessageAsync(existingMessage);


        //    //    var button3 = new DiscordButtonComponent(ButtonStyle.Primary, "button_3", "Button 3");
        //    //    var button4 = new DiscordButtonComponent(ButtonStyle.Primary, "button_4", "Button 4");
        //    //    var components = new DiscordComponent[] { button1, button2, button3, button4 };

        //    //    DiscordMessageBuilder messageBuilder = new(existingMessage);
        //    //    messageBuilder.ClearComponents();
        //    //    messageBuilder.AddComponents(components);

        //    //    var response = await existingMessage.ModifyAsync(messageBuilder);
        //    //}

        //    //// Handle the result
        //    //if (result.TimedOut)
        //    //{
        //    //    await ctx.FollowUpAsync(new DiscordFollowupMessageBuilder().WithContent("You didn't select an option in time!"));
        //    //}
        //    //else
        //    //{
        //    //    await ctx.FollowUpAsync(new DiscordFollowupMessageBuilder().WithContent($"You selected {result.Result.Values.First()}!"));
        //    //}
        //    //await ctx.DeferAsync(true);
        //    //CreateUserRequest newUser = new()
        //    //{
        //    //    UserId = ctx.Member.Id.ToString(),
        //    //    UserName = ctx.Member.Username,
        //    //    Key = key
        //    //};
        //    //var response = await _userClient.RegisterUserAsync(newUser);

        //    //var embedMessage = new DiscordEmbedBuilder
        //    //{
        //    //    Color = response.Success ? DiscordColor.Green : DiscordColor.Red,
        //    //    Title = response.Success ? "Registration Successful" : "Registration Failed",
        //    //    Description = response.Message
        //    //};

        //    //await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(embedMessage));

        //}

        [SlashCommand("publish-subscribe-message", "Publish the subscribe message with the option for the user to choose")]
        public async Task PublishSubscribeMessageCommand(InteractionContext ctx)
        {
            try
            {
                //await ctx.DeferAsync();
                //var userId = ctx.Member.Id; // The user's ID
                string mention = $"<@1012125338777681920>"; // Constructing the mention manually
                var embed = EmbedHelper.CreateSubscriptionEndingSoonEmbed(DateTime.UtcNow, 3);

                var message = new DiscordMessageBuilder()
                    .AddEmbed(embed);
                await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder(message).WithContent($"{mention}, your subscription is ending soon!"));

                //var messageBuilder = CreateSubscriptionMessage();
                //if (!messageId.HasValue)
                //{
                //    await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder(messageBuilder));
                //}
                //else
                //{
                //    // Acknowledge the interaction immediately
                //    await ctx.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource);
                //    var channel = ctx.Channel;
                //    var existingMessage = await channel.GetMessageAsync(Convert.ToUInt64(messageId));

                //    await existingMessage.ModifyAsync(messageBuilder);
                //    await ctx.FollowUpAsync(new DiscordFollowupMessageBuilder().WithContent("Message updated successfully.").AsEphemeral(true));
                //}
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            //try
            //{
            //    var walletSubmissionButton = new DiscordButtonComponent(ButtonStyle.Success, "wallet_submission_btn", "Submit Wallet");
            //    var subscribeButton = new DiscordButtonComponent(ButtonStyle.Primary, "subscribe_btn", "Subscribe");
            //    var planDetailsButton = new DiscordButtonComponent(ButtonStyle.Secondary, "plan_details_btn", "Plan Details");

            //    StringBuilder instructions = new();
            //    instructions.AppendLine("1. Register the wallet you will use for payment.");
            //    instructions.AppendLine("2. Click the 'Subscribe' button to initiate the process.");
            //    instructions.AppendLine("3. Send the requested amount from the wallet registered in Step 1.");
            //    instructions.AppendLine("4. Enjoy our 100% AFK bot service!");

            //    var message = new DiscordMessageBuilder()
            //        .AddEmbed(new DiscordEmbedBuilder()
            //              .WithTitle("Become a privileged member")
            //              //.WithDescription("Welcome to Protools server. Please follow these steps to complete your subscription")
            //              //.AddField("Prerequisite", "- Must have Alphabot Premium.")
            //              .AddField("Instructions", instructions.ToString())
            //              .WithColor(DiscordColor.Gold)
            //              .WithTimestamp(DateTime.UtcNow)
            //              .WithFooter("Pro Payments"))
            //        .AddComponents(subscribeButton, walletSubmissionButton, planDetailsButton);

            //    await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder(message));
            //}
            //catch (Exception e)
            //{
            //    Console.WriteLine(e.Message);
            //}
        }

        //[SlashCommand("embed", "Send an embed")]
        //public async Task SendEmbed(InteractionContext ctx)
        //{
        //    var embed = new DiscordEmbedBuilder
        //    {
        //        Title = "Embed Title",
        //        Description = "Embed Description",
        //        Color = DiscordColor.Blue // Customize color if needed
        //    };

        //    await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
        //                                                 new DiscordInteractionResponseBuilder().AddEmbed(embed));
        //    // Delete the response after a certain time or condition
        //    await Task.Delay(TimeSpan.FromSeconds(5)); // Example delay

        //    await ctx.DeleteResponseAsync();
        //}
    }
}
