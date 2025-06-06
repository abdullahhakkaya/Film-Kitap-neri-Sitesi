namespace Proje.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Role { get; set; } 

        public DateTime CreatedAt { get; private set; }

        public User()
        {
            CreatedAt = DateTime.Now;
        }
    }
}