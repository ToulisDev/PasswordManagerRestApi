namespace PasswordAppAPI.Models
{
    public partial class Password
    {
        public int PasswordsId { get; set; }
        public string PasswordsSite { get; set; } = null!;
        public string PasswordsUsername { get; set; } = null!;
        public string PasswordsPassword { get; set; } = null!;
        public Guid UserId { get; set; }
    }
}
