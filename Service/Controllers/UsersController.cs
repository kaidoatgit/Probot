using Microsoft.AspNetCore.Mvc;
using ProPayments.Service.Dtos.ProductKeys.Response;
using ProPayments.Service.Dtos.Users.Request;
using ProPayments.Service.Dtos.Users.Response;
using ProPayments.Service.Mappers;
using ProPayments.Service.Services.Services.IServices;

namespace ProPayments.Service.Controllers
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

        [HttpPut("{id}/wallet")]
        public async Task<IActionResult> UpdateWalletAddressAsync(ulong id, [FromBody] UpdateWalletRequest request)
        {
            await _userService.UpdateWalletAddressAsync(id, request.WalletAddress);
            return NoContent();
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserAsync(ulong id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            UserResponse userResponse = _mapper.MapToUserResponse(user);
            return Ok(userResponse);
        }        

        [HttpGet("with-subscriptions")]
        public async Task<IActionResult> GetUsersWithSubscriptionsAsync()
        {
            var users = await _userService.GetUsersWithSubscriptionsAsync();
            IEnumerable<UserWithSubscriptionsResponse> usersWithSubscriptionsResponse = users
                .Select(u => _mapper.MapToUserWithSubscriptionsResponse(u));
            return Ok(usersWithSubscriptionsResponse);
        }

        [HttpGet("{userId}/product-keys/{code}")]
        public async Task<IActionResult> GetProductKeyAsync(ulong userId, string code, [FromQuery] bool isActivated)
        {
            var productKey = await _userService.GetProductKeyAsync(userId, code, isActivated);
            ProductKeyResponse productKeyResponse = _mapper.MapToProductKeyResponse(productKey);
            return Ok(productKeyResponse);
        }


        [HttpGet("{userId}/product-keys")]
        public async Task<IActionResult> GetProductKeysAsync(ulong userId, [FromQuery] bool isActivated)
        {
            var productKeys = await _userService.GetProductKeysAsync(userId, isActivated);
            IEnumerable<ProductKeyResponse> productKeyResponses = productKeys
                    .Select(pk => _mapper.MapToProductKeyResponse(pk));
            return Ok(productKeyResponses);
        }
    }
}
