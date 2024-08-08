using Microsoft.EntityFrameworkCore;
using ProPayments.Service.Data;
using ProPayments.Service.Data.Entities;
using ProPayments.Service.Dtos.Plans.Request;
using ProPayments.Service.Dtos.Plans.Response;
using ProPayments.Service.Exceptions;
using ProPayments.Service.Mappers;
using ProPayments.Service.Services.Services.IServices;

namespace ProPayments.Service.Services.Services
{
    public class PlanService : IPlanService
    {
        private readonly SubscriptionContext _context;
        private readonly Mapper _mapper;

        public PlanService(SubscriptionContext context, Mapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<Plan> GetPlanByIdAsync(int planId)
        {
            var plan = await _context.Plans.FindAsync(planId);
            if (plan == null) throw new ServiceException(StatusCodes.Status404NotFound, $"Plan {planId} not found");
            return plan;
        }

        public async Task<IEnumerable<Plan>> GetPlansAsync()
        {
            return await _context.Plans.ToListAsync();
        }

        public async Task<IEnumerable<PlanOption>> GetPlansOptionsAsync()
        {
            var planOptions = await _context.PlanOptions.ToListAsync();
            return planOptions;
        }

        public async Task<IEnumerable<PlanWithOptionsResponse>> GetPlansWithOptionsAsync()
        {
            var plans = await _context.PlanOptions
                .Include(p => p.Plan)
                .ToListAsync();

            var optionsGroupedByPlan = plans
             .GroupBy(p => p.Plan) // Group by Plan
             .Select(g => new PlanWithOptionsResponse
             {
                 Id = g.Key!.Id,
                 RoleId = g.Key.RoleId,
                 Type = g.Key.Type,
                 Description = g.Key.Description,
                 PlanOptions = g.Select(p => new PlanOptionResponse
                 {
                     Id = p.Id,
                     Period = p.Period,
                     Price = p.Price,
                     PeriodDescription = p.PeriodDescription
                 }).ToList()
             })
             .ToList(); // Perform grouping and projection in memory

            return optionsGroupedByPlan;
        }

        public async Task<bool> UpdatePlansRoleIdAsync(IEnumerable<UpdatePlanRoleIdRequest> request)
        {
            bool isModified = false;

            var plans = request.Select(p => _mapper.MapToPlanEntity(p));
            var dbPlans = await _context.Plans.ToListAsync();

            foreach (var dbPlan in dbPlans)
            {
                // Find the corresponding update request for the current plan type
                var planRequest = plans.FirstOrDefault(p => p.Type == dbPlan.Type);

                // If there's a matching update request
                if (planRequest != null)
                {
                    // Update DiscordRoleId if it's null or different
                    if (dbPlan.RoleId == null || dbPlan.RoleId != planRequest.RoleId)
                    {
                        dbPlan.RoleId = planRequest.RoleId;
                        isModified = true;
                    }
                }
            }

            if (isModified)
            {
                await _context.SaveChangesAsync();
            }
            return isModified;
        }
    }
}
