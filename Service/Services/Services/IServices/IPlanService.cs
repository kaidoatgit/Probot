using ProPayments.Service.Data.Entities;
using ProPayments.Service.Dtos.Plans.Request;
using ProPayments.Service.Dtos.Plans.Response;

namespace ProPayments.Service.Services.Services.IServices
{
    public interface IPlanService
    {
        Task<IEnumerable<Plan>> GetPlansAsync();
        Task<Plan> GetPlanByIdAsync(int planId);

        Task<IEnumerable<PlanOption>> GetPlansOptionsAsync();
        Task<IEnumerable<PlanWithOptionsResponse>> GetPlansWithOptionsAsync();
        Task<bool> UpdatePlansRoleIdAsync(IEnumerable<UpdatePlanRoleIdRequest> request);

    }
}
