using Microsoft.AspNetCore.Mvc;
using PasswordAppAPI.Models;
using PasswordAppAPI.Repositories;

namespace PasswordAppAPI.Controllers
{
    [Route("passwordApp/auth/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUserRepository _repository;


        public AuthController(IUserRepository repository)
        {
            _repository = repository;
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterUser(UserDTO request)
        {
            if (request == null)
            {
                return BadRequest("Request is Empty!");
            }
            var response = await _repository.RegisterUser(request);
            if (response > 0) { return Ok("User successfully Registered"); }
            return BadRequest("Username already Exists");
        }

        [HttpPost("login")]
        public async Task<IActionResult> LoginUser(UserDTO request)
        {
            if (request != null)
            {
                var authorization = await _repository.LoginUser(request);
                if (authorization != null) { return Ok(authorization); }
            }
            return BadRequest("Username or Password are not valid.");
        }

    }
}
