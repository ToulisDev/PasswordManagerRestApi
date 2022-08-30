using Microsoft.AspNetCore.Mvc;
using PasswordAppAPI.Models;

namespace PasswordAppAPI.Repositories
{
    public interface IUserRepository
    {
        Task<int> RegisterUser(UserDTO request);
        Task<ActionResult<string>?> LoginUser(UserDTO request);
    }
}
