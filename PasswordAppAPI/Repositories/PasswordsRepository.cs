using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using PasswordAppAPI.Models;

namespace PasswordAppAPI.Repositories
{
    public class PasswordsRepository : IPasswordsRepository
    {
        private readonly passwordAppContext _context;
        private static Password password = new Password();

        public PasswordsRepository(passwordAppContext context)
        {
            _context = context;
        }

        public async Task<int> CreatePassword(string UserID, PasswordDTO pass)
        {
            password.PasswordsSite = pass.PasswordsSite;
            password.PasswordsUsername = pass.PasswordsUsername;
            password.PasswordsPassword = pass.PasswordsPassword;
            password.UserId = new Guid(UserID);

            var passSiteParam = new SqlParameter("@PASSWORDS_SITE", password.PasswordsSite);
            var passUsernameParam = new SqlParameter("@PASSWORDS_USERNAME", password.PasswordsUsername);
            var passPasswordParam = new SqlParameter("@PASSWORDS_PASSWORD", password.PasswordsPassword);
            var passUserIdParam = new SqlParameter("@USER_ID", password.UserId);
            var listPassword = _context
                            .Passwords
                            .FromSqlRaw("exec existantPasswordNote @PASSWORDS_SITE, @USER_ID", passSiteParam, passUserIdParam)
                            .ToList();
            if(listPassword.Count > 0)
            {
                return 0;
            }

            await _context.Database.ExecuteSqlRawAsync(
                    "exec createPassword @PASSWORDS_SITE, @PASSWORDS_USERNAME, @PASSWORDS_PASSWORD, @USER_ID",
                    passSiteParam, passUsernameParam, passPasswordParam, passUserIdParam);

            return 1;
        }

        public async Task<int> DeletePassword(string UserID, string PasswordID)
        {
            password.PasswordsId = int.Parse(PasswordID);
            password.UserId = new Guid(UserID);

            var passIDParam = new SqlParameter("@PASSWORDS_ID", password.PasswordsId);
            var passUserIdParam = new SqlParameter("@USER_ID", password.UserId);
            try
            {
                await _context.Database.ExecuteSqlRawAsync(
                        "exec deletePasswordNote @PASSWORDS_ID, @USER_ID",
                        passIDParam, passUserIdParam);
            }catch(Exception e)
            {
                Console.WriteLine(e);
                return 0;
            }
            return 1;
        }

        public async Task<ActionResult<Password>> GetPassword(string UserID, string PasswordID)
        {
            password.PasswordsId = int.Parse(PasswordID);
            password.UserId = new Guid(UserID);

            var passIDParam = new SqlParameter("@PASSWORDS_ID", password.PasswordsId);
            var passUserIdParam = new SqlParameter("@USER_ID", password.UserId);
            var listPasswordAsync = _context
                            .Passwords
                            .FromSqlRaw("exec getSpecPassword @PASSWORDS_ID, @USER_ID", passIDParam, passUserIdParam)
                            .ToListAsync();
            List<Password> listPasswords = await listPasswordAsync;
            return listPasswords.First();
        }

        public async Task<ActionResult<IEnumerable<Password>>> GetPasswords(string UserID)
        {
            password.UserId = new Guid(UserID);

            var passUserIdParam = new SqlParameter("@USER_ID", password.UserId);
            var listPassword = _context
                            .Passwords
                            .FromSqlRaw("exec getallPasswords  @USER_ID", passUserIdParam)
                            .ToListAsync();
            
            return await listPassword;
        }

        public async Task<int> UpdatePassword(string UserID, string PasswordID, PasswordDTO pass)
        {
            password.PasswordsId = int.Parse(PasswordID);
            password.PasswordsSite = pass.PasswordsSite;
            password.PasswordsUsername = pass.PasswordsUsername;
            password.PasswordsPassword = pass.PasswordsPassword;
            password.UserId = new Guid(UserID);

            var passIDParam = new SqlParameter("@PASSWORDS_ID", password.PasswordsId);
            var passSiteParam = new SqlParameter("@PASSWORDS_SITE", password.PasswordsSite);
            var passUsernameParam = new SqlParameter("@PASSWORDS_USERNAME", password.PasswordsUsername);
            var passPasswordParam = new SqlParameter("@PASSWORDS_PASSWORD", password.PasswordsPassword);
            var passUserIdParam = new SqlParameter("@USER_ID", password.UserId);
            try
            {
                await _context.Database.ExecuteSqlRawAsync(
                        "exec updatePassword @PASSWORDS_ID, @PASSWORDS_SITE, @PASSWORDS_USERNAME, @PASSWORDS_PASSWORD, @USER_ID",
                        passIDParam, passSiteParam, passUsernameParam, passPasswordParam, passUserIdParam);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return 0;
            }
            return 1;
        }
    }
}
