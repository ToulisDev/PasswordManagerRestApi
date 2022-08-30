using Isopoh.Cryptography.Argon2;
using Isopoh.Cryptography.SecureArray;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using PasswordAppAPI.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;

namespace PasswordAppAPI.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly passwordAppContext context;
        private static User user = new User();
        private static readonly RandomNumberGenerator Rng = RandomNumberGenerator.Create();
        private readonly IConfiguration _configuration;

        //Initialize Context for db communication
        public UserRepository(passwordAppContext context, IConfiguration configuration)
        {
            this.context = context;
            _configuration = configuration;
        }

        //All Tasks
        public async Task<int> RegisterUser(UserDTO request)
        {
            user.Username = request.Username;
            var usernameParam = new SqlParameter("@USERNAME", user.Username);

            var users = context
                    .Users
                    .FromSqlRaw("exec validateLoginCreds @USERNAME", usernameParam)
                    .ToList();
            if(users.Count > 0)
            {
                return 0;
            }
            else
            {
                user.UserId = Guid.NewGuid();
                user.Password = CreatePasswordHash(request.Password, user.Username, user.UserId.ToString());
                user.InsertDate = DateTime.Now;

                var userIDParam = new SqlParameter("@USER_ID", user.UserId);
                var passwordParam = new SqlParameter("@PASSWORD", user.Password);
                var insertDateParam = new SqlParameter("@INSERT_DATE", user.InsertDate);

                await context.Database.ExecuteSqlRawAsync(
                    "exec registerLoginCreds @USER_ID, @USERNAME, @PASSWORD, @INSERT_DATE",
                    userIDParam,usernameParam,passwordParam,insertDateParam);
                return 1;
            }
        }

        public async Task<ActionResult<string>?> LoginUser(UserDTO request)
        {

            string hashedPass;
            var usernameParam = new SqlParameter("@USERNAME",request.Username);
            Task<string> tokenTask = CreateToken(request.Username);
            List<User> hashedPasswords = await context
                            .Users
                            .FromSqlRaw("exec validateLoginCreds @USERNAME", usernameParam)
                            .ToListAsync();
            if(hashedPasswords.Count > 0)
            {
                hashedPass = hashedPasswords.First().Password;

                if (verifyPasswordHash(request.Password, hashedPass))
                {
                    string token = await tokenTask;
                    if(token != string.Empty) { return token; }
                }
            }
            return null;
        }

        //Methods For helping process
        private string CreatePasswordHash(string password, string username, string id)
        {
            byte[] idBytes = System.Text.Encoding.UTF8.GetBytes(id);
            byte[] usernameBytes = System.Text.Encoding.UTF8.GetBytes(username);
            byte[] passwordBytes = System.Text.Encoding.UTF8.GetBytes(password);
            byte[] salt = new byte[20];

            Rng.GetBytes(salt);

            var config = new Argon2Config
            {
                Type = Argon2Type.DataIndependentAddressing,
                Version = Argon2Version.Nineteen,
                TimeCost = 10,
                MemoryCost = 32768,
                Lanes = 5,
                Threads = Environment.ProcessorCount,
                Password = passwordBytes,
                Salt = salt,
                HashLength = 40
            };
            var argon2A = new Argon2(config);
            string hashedPassword;
            using (SecureArray<byte> hashA = argon2A.Hash())
            {
                hashedPassword = config.EncodeString(hashA.Buffer);
            }
            return hashedPassword;
        }

        private bool verifyPasswordHash(string pass,string hashedPass)
        {
            byte[] bytePass = System.Text.Encoding.UTF8.GetBytes(pass);
            if(Argon2.Verify(hashedPass, bytePass, 5))
            {
                return true;
            }

            return false;
        }

        private Task<string> CreateToken(string username)
        {
            var usernameParam = new SqlParameter("@USERNAME", username);
            var users = context
                    .Users
                    .FromSqlRaw("exec validateLoginCreds @USERNAME", usernameParam)
                    .ToList();
            if (users.Count > 0)
            {
                Guid userIdG = users.First().UserId;
                string userId = userIdG.ToString();
                List<Claim> claimsL = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, userId)
                };

                var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(
                    _configuration.GetSection("JWTSettings:SecretKey").Value));

                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

                var token = new JwtSecurityToken(
                    claims: claimsL,
                    expires: DateTime.UtcNow.AddMinutes(30),
                    signingCredentials: creds);
                var jwt = new JwtSecurityTokenHandler().WriteToken(token);
                return Task.FromResult(jwt);
            }
            return Task.FromResult(string.Empty);
        }
    }
}
