namespace PasswordAppAPI.Models
{
    public partial class User
    {
        public User()
        {
            Passwords = new HashSet<Password>();
        }

        public Guid UserId { get; set; }
        public string Username { get; set; } = null!;
        public string Password { get; set; } = null!;
        public DateTime? InsertDate { get; set; }

        public virtual ICollection<Password> Passwords { get; set; }
    }
}
