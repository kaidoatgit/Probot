using Microsoft.AspNetCore.Mvc;
using ProPayments.Service.Dtos.Plans.Request;
using ProPayments.Service.Dtos.Plans.Response;
using ProPayments.Service.Mappers;
using ProPayments.Service.Services.Services.IServices;

namespace ProPayments.Service.Controllers
{
    [Route("api/plans")]
    [ApiController]
    public class PlansController : ControllerBase
    {
        private readonly IPlanService _planService;
        private readonly Mapper _mapper;
        public PlansController(IPlanService planService, Mapper mapper)
        {
            _planService = planService;
            _mapper = mapper;
        }

        [HttpGet("with_options")]
        public async Task<IActionResult> GetPlansWithOptionsAsync()
        {
            var plansWithOptions = await _planService.GetPlansWithOptionsAsync();
            return Ok(plansWithOptions);
        }

        [HttpPost("roleId")]
        public async Task<IActionResult> UpdatePlansRoleIdAsync(IEnumerable<UpdatePlanRoleIdRequest> request)
        {
            var isModified = await _planService.UpdatePlansRoleIdAsync(request);
            return isModified ? NoContent() : StatusCode(304); //304 = not modified
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPlanByIdAsync(int id)
        {
            var plan = await _planService.GetPlanByIdAsync(id);
            PlanResponse planResponse = _mapper.MapToPlanResponse(plan);
            return Ok(planResponse);
        }

        [HttpGet]
        public async Task<IActionResult> GetPlansAsync()
        {
            var plans = await _planService.GetPlansAsync();
            List<PlanResponse> plansResponse = plans.Select(p => _mapper.MapToPlanResponse(p)).ToList();
            return Ok(plansResponse);
        }

        [HttpGet("options")]
        public async Task<IActionResult> GetPlansOptionsAsync()
        {
            var planOptions = await _planService.GetPlansOptionsAsync();
            List<PlanOptionResponse> plansOptionsResponse = planOptions.Select(p => _mapper.MapToPlanOptionResponse(p)).ToList();
            return Ok(plansOptionsResponse);
        }

        
    }
}
