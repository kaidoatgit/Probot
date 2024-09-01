using DSharpPlus;
using DSharpPlus.Entities;
using Probot.Client.Models;

namespace Probot.Client.Services.Managers
{
    public class DiscordManager
    {
        private readonly SemaphoreSlim _concurrentMemberUpdate = new(1, 1);

        public async Task UpdateDiscordMemberAsync(DiscordMember member, IReadOnlyDictionary<ulong, DiscordRole> roles, Metrics? metrics)
        {
            await _concurrentMemberUpdate.WaitAsync();
            try
            {
                if (metrics == null)
                {
                    return;
                }

                foreach (var (roleId, role) in roles)
                {
                    var inactiveKeysCount = metrics.InactiveKeysPerProduct.GetValueOrDefault(roleId);
                    var activeSubsCount = metrics.ActiveSubsPerProduct.GetValueOrDefault(roleId);
                    if((inactiveKeysCount > 0 || activeSubsCount > 0) && !member.Roles.Contains(role))
                    {
                        await member.GrantRoleAsync(role);
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
