using Microsoft.AspNetCore.SignalR;
using ProPayments.Service.Services.Hubs.IClients;

namespace ProPayments.Service.Services.Hubs
{
    public class NotificationHub : Hub<INotificationClient>
    {
    }
}
