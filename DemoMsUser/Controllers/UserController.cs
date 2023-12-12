using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using DemoMsUser.Common.Constants;
using DemoMsUser.Common.Exceptions;
using DemoMsUser.Common.Responses;
using DemoMsUser.Dtos;
using DemoMsUser.Interfaces;

namespace DemoMsUser.Controllers
{
    [Authorize]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ITokenService _tokenService;
        private readonly ILogger<UserController> _logger;

        public UserController(
            IUserService userService,
            ITokenService tokenService,
            ILogger<UserController> logger
        )
        {
            _logger = logger;
            _tokenService = tokenService;
            _userService = userService;
        }

        //GET
        [HttpGet("api/users")]
        [Authorize(Roles = UserRoles.Admin)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<UserGetShortDto>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorDetails))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ErrorDetails))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ErrorDetails))]
        public async Task<IActionResult> GetAll()
        {
            var response = await _userService.GetAll();
            return StatusCode(response.StatusCode, response.Value);
        }

        //GET
        [HttpGet("api/users/profile")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UserGetDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorDetails))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ErrorDetails))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ErrorDetails))]
        public async Task<IActionResult> GetSelf()
        {
            var userId = _tokenService.GetSubFromToken();

            // redundant?
            if (userId is null)
                throw new InvalidInputException(StatusMessages.InvalidSubField);

            var response = await _userService.GetUser(userId, null, null);
            return StatusCode(response.StatusCode, response.Value);
        }

        //GET
        [HttpGet("api/users/find")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<UserGetShortDto>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorDetails))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ErrorDetails))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ErrorDetails))]
        public async Task<IActionResult> FindBy(
            [FromQuery] string? username,
            [FromQuery] string? email,
            [FromQuery] bool? issubstr)
        {
            var response = email is not null || issubstr is null || issubstr == false
                ? await _userService.GetUser(null, username, email)
                : await _userService.GetUsersBySubstring(username, email);
            return StatusCode(response.StatusCode, response.Value);
        }

        [HttpGet("api/users/{userId:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<UserGetDto>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorDetails))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ErrorDetails))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ErrorDetails))]
        public async Task<IActionResult> GetById([FromRoute] string userId)
        {
            var response = await _userService.GetUser(userId, null, null);
            return StatusCode(response.StatusCode, response.Value);
        }

        //POST
        [HttpPost("api/users")]
        [Authorize(Roles = UserRoles.Admin)]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(List<UserPostDto>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorDetails))]
        [ProducesResponseType(StatusCodes.Status409Conflict, Type = typeof(ErrorDetails))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ErrorDetails))]
        public async Task<IActionResult> AddUsers([FromBody] List<UserPostDto> userDtos)
        {
            var response = await _userService.AddUsers(userDtos);
            return StatusCode(response.StatusCode, response.Value);
        }

        //POST
        [HttpPost("api/users/{userId:guid}")]
        [Authorize(Policy = "AdminOnly, SubIdMatch")]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(List<UserPostDto>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorDetails))]
        [ProducesResponseType(StatusCodes.Status409Conflict, Type = typeof(ErrorDetails))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ErrorDetails))]
        public async Task<IActionResult> AddUser(
            [FromRoute] string userId,
            [FromBody] UserPostDto userDto)
        {
            var response = await _userService.AddUser(userId, userDto);
            return StatusCode(response.StatusCode, response.Value);
        }

        //POST
        [HttpPost("api/users/validate")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorDetails))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ErrorDetails))]
        public async Task<IActionResult> ValidateUsers([FromBody] List<string> userIds)
        {
            var response = await _userService.ValidateUsersExist(userIds);
            return StatusCode(response.StatusCode, response.Value);
        }

        //PATCH
        [HttpPatch("api/users")]
        [Authorize(Roles = UserRoles.Admin)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<UserPatchDto>))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ErrorDetails))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ErrorDetails))]
        public async Task<IActionResult> UpdateUsers([FromBody] List<UserPatchDtoAdmin> userDtos)
        {
            var response = await _userService.UpdateUsers(userDtos);
            return StatusCode(response.StatusCode, response.Value);
        }

        //PATCH
        [HttpPatch("api/users/{userId:guid}")]
        [Authorize(Policy = "AdminOnly, SubIdMatch")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<UserPatchDto>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorDetails))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ErrorDetails))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ErrorDetails))]
        public async Task<IActionResult> UpdateUser(
            [FromRoute] string userId,
            [FromBody] UserPatchDto userDto)
        {
            var response = await _userService.UpdateUser(userId, userDto);
            return StatusCode(response.StatusCode, response.Value);
        }

        //DELETE
        [HttpDelete("api/users")]
        [Authorize(Roles = UserRoles.Admin)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorDetails))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ErrorDetails))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ErrorDetails))]
        public async Task<IActionResult> DeleteUsers([FromBody] List<string> userIds)
        {
            var response = await _userService.DeleteUsers(userIds);
            return StatusCode(response.StatusCode, response.Value);
        }

        //DELETE
        [HttpDelete("api/users/{userId:guid}")]
        [Authorize(Policy = "AdminOnly, SubIdMatch")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorDetails))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ErrorDetails))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ErrorDetails))]
        public async Task<IActionResult> DeleteUser([FromRoute] string userId)
        {
            var response = await _userService.SoftDeleteUser(userId);
            return StatusCode(response.StatusCode, response.Value);
        }
    }
}
