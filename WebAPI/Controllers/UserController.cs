using Application.Contracts;
using Application.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUser _user;
        public UserController(IUser user)
        {
            this._user = user;
        }

        [HttpPost("login")]
        public async Task<ActionResult<LoginResponse>> LogUserIn(LoginDTO loginDTO)
        {
            var result = await _user.LoginUserAsync(loginDTO);
            if (!result.Flag)
            {
                return Unauthorized(result);
            }
            return Ok(result);
        }

        [HttpPost("register")]
        public async Task<ActionResult<LoginResponse>> RegisterUser(RegisterUserDto registerUserDto)
        {
            var result = await _user.RegisterUserAsync(registerUserDto);
            return Ok(result);
        }
    }
}
