using Microsoft.AspNetCore.Mvc;
using PasswordAppAPI.Models;

namespace PasswordAppAPI.Repositories
{
    public interface IPasswordsRepository
    {
        Task<ActionResult<IEnumerable<Password>>> GetPasswords(string UserID);
        Task<ActionResult<Password>> GetPassword(string UserID, string PasswordID);
        Task<int> CreatePassword(string UserID, PasswordDTO pass);
        Task<int> DeletePassword(string UserID, string PasswordID);
        Task<int> UpdatePassword(string UserID, string PasswordID, PasswordDTO pass);
    }
}
