using Microsoft.AspNetCore.Mvc;
using Probot.SubscriptionApi.Mappers;
using Probot.SubscriptionApi.Services.Services.IServices;
using Probot.Shared.Dtos.User.Request;
using Probot.Shared.Dtos.User.Response;

namespace Probot.SubscriptionApi.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly Mapper _mapper;

        public UsersController(IUserService userService, Mapper mapper)
        {
            _userService = userService;
            _mapper = mapper;
        }

        [HttpPost]
        public async Task<IActionResult> CreateUserAsync([FromBody] UserRequest request)
        {
            var user = await _userService.CreateUserAsync(request);
            UserResponse userResponse = _mapper.MapToUserResponse(user);
            return CreatedAtAction(nameof(GetUserAsync), new { id = userResponse.Id }, userResponse);
        }
        
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserAsync(ulong id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            UserResponse userResponse = _mapper.MapToUserResponse(user);
            return Ok(userResponse);
        }

        [HttpPut("{id}/wallet")]
        public async Task<IActionResult> UpdateWalletAddressAsync(ulong id, [FromBody] UpdateWalletRequest request)
        {
            await _userService.UpdateWalletAddressAsync(id, request.WalletAddress);
            return NoContent();
        }

        [HttpGet("with-metrics")]
        public async Task<IActionResult> GetUsersWithMetricsAsync()
        {
            var users = await _userService.GetUsersWithMetricsAsync(CancellationToken.None);
            IEnumerable<UserMetricsResponse> usersMetricsResponse = users.Select(u => _mapper.MapToUserMetricsResponse(u));
            return Ok(usersMetricsResponse);
        }
    }
}
