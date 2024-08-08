using DSharpPlus;
using DSharpPlus.Entities;
using ProPayments.Client.Models;

namespace ProPayments.Client.Services.Managers
{
    public class DiscordManager
    {
        private readonly SemaphoreSlim _concurrentMemberUpdate = new(1, 1);

        public async Task UpdateDiscordMemberAsync(DiscordMember member, IReadOnlyDictionary<ulong, DiscordRole> roles, List<Subscription>? subscriptions)
        {
            await _concurrentMemberUpdate.WaitAsync();
            try
            {
                if (subscriptions == null)
                {
                    return;
                }

                foreach (var subscription in subscriptions)
                {
                    if (roles.TryGetValue(subscription.PlanRoleId, out var role))
                    {
                        if (!member.Roles.Contains(role))
                        {
                            await member.GrantRoleAsync(role);
                        }
                    }
                }
                await Task.Delay(TimeSpan.FromSeconds(1));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing role assignment: {ex.Message}");
            }
            finally
            {
                _concurrentMemberUpdate.Release();
            }
        }


    }
}
