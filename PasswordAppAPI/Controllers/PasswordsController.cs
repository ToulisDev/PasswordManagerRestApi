using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PasswordAppAPI.Models;
using PasswordAppAPI.Repositories;

namespace PasswordAppAPI.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("passwordApp/api/[controller]")]
    [ApiController]
    public class PasswordsController : ControllerBase
    {
        private readonly IPasswordsRepository _repository;

        public PasswordsController(IPasswordsRepository repository)
        {
            _repository = repository;
        }

        // Post: api/Passwords
        [HttpPost("GetPasswords")]
        public async Task<ActionResult<IEnumerable<Password>>> GetPasswords()
        {
            if (User.Identity == null)
            {
                return BadRequest("No authentication.");
            }
            if (User.Identity.Name != null)
            {
                var userId = User.Identity.Name;
                var result = await _repository.GetPasswords(userId);

                return result;
            }
            return BadRequest("No authentication");
        }

        // Post: api/Passwords/5
        [HttpPost("GetPassword")]
        public async Task<ActionResult<Password>> GetPassword(string passId)
        {
            if (User.Identity == null)
            {
                return BadRequest("No authentication.");
            }
            if (User.Identity.Name != null)
            {
                var userId = User.Identity.Name;
                var result = await _repository.GetPassword(userId, passId);

                return result;
            }
            return BadRequest("No authentication");
        }

        // POST: api/Passwords/
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost("UpdatePassword")]
        public async Task<IActionResult> UpdatePassword(string passId, PasswordDTO password)
        {
            if (User.Identity == null)
            {
                return BadRequest("No authentication.");
            }
            if (User.Identity.Name != null)
            {
                var userId = User.Identity.Name;
                var result = await _repository.UpdatePassword(userId, passId, password);
                if (result > 0) { return Ok("Info: Password Successfully Updated."); }

                return BadRequest("Error: Password doesn't Exist.");
            }
            return BadRequest("No authentication");
        }

        // POST: api/Passwords
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost("CreatePassword")]
        public async Task<IActionResult> CreatePassword(PasswordDTO password)
        {
            if (User.Identity == null)
            {
                return BadRequest("No authentication.");
            }
            if (User.Identity.Name != null)
            {
                var userId = User.Identity.Name;
                var result = await _repository.CreatePassword(userId, password);
                if (result > 0)
                {
                    return Ok("Info: Password Successfully Created.");
                }
                return Problem("Error: Site Name Already Exists.");
            }
            return BadRequest("No authentication");
        }

        // POST: api/Passwords
        [HttpPost("DeletePassword")]
        public async Task<IActionResult> DeletePassword(string passwordId)
        {
            if (User.Identity == null)
            {
                return BadRequest("No authentication.");
            }
            if (User.Identity.Name != null)
            {
                var userId = User.Identity.Name;
                int result = await _repository.DeletePassword(userId, passwordId);
                if (result > 0)
                {
                    return Ok("Info: Password Successsfully Deleted.");
                }
                return NotFound("Error: Password Not Found.");
            }
            return BadRequest("No authentication");
    }
    }
}
