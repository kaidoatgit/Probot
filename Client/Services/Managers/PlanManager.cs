using DSharpPlus.Entities;
using ProPayments.Client.Clients.ProPayments;
using ProPayments.Client.Mappers;
using ProPayments.Client.Models;

namespace ProPayments.Client.Services.Managers
{
    public class PlanManager
    {
        public List<Plan> _plans = new();
        private readonly PlanClient _planClient;
        private readonly Mapper _mapper;

        public PlanManager(PlanClient planClient, Mapper mapper)
        {
            _planClient = planClient;
            _mapper = mapper;
            Console.WriteLine("Plan Manager created");
        }

        public List<Plan> Plans => _plans;

        public async Task<bool> LoadPlansAsync(List<DiscordRole> guildRoles)
        {
            _plans.Clear();
            var apiResponse = await _planClient.GetPlansWithOptionsAsync();
            if(apiResponse.Data == null)
            {
                return false;
            }

            //for the moment is a must to have plans, but maybe in future could be an options for admins to add plans
            if (!apiResponse.Data.Any())
            {
                return false;
            }

            var plans = apiResponse.Data.Select(p => _mapper.MapToPlan(p)).ToList();
            var plansToUpdate = new List<Plan>();
            foreach (var role in guildRoles)
            {
                var matchedPlan = plans.FirstOrDefault(p => p.Type.ToString() == role.Name);
                if (matchedPlan != null)
                {
                    if(matchedPlan.RoleId != role.Id)
                    {
                        matchedPlan.RoleId = role.Id;
                        plansToUpdate.Add(matchedPlan);
                    }
                    _plans.Add(matchedPlan);
                }
            }

            if (!plansToUpdate.Any())
            {
                return true;
            }
            return await UpdatePlansRoleIdAsync(plansToUpdate);
        }

        private async Task<bool> UpdatePlansRoleIdAsync(List<Plan> plans)
        {
            var plansRoleIdRequest = _mapper.MapToUpdatePlansRoleIdRequest(plans);
            return await _planClient.UpdatePlansRoleIdAsync(plansRoleIdRequest);
        }

        public Plan? GetPlanByRoleId(string roleId)
        {
            Plan? plan = Plans.FirstOrDefault(p => p.RoleId.ToString()!.Equals(roleId));
            return plan;
        }

        public List<PlanOption>? GetDurationsWithPrices(string roleId)
        {
            Plan? plan = Plans.FirstOrDefault(p => p.RoleId.ToString()!.Equals(roleId));
            return plan?.PlanOptions;
        }

    }
}
