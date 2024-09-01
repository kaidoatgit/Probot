using Microsoft.AspNetCore.SignalR;
using Probot.SubscriptionApi.Services.Hubs.IClients;

namespace Probot.SubscriptionApi.Services.Hubs
{
    public class NotificationHub : Hub<INotificationClient>
    {
    }
}
