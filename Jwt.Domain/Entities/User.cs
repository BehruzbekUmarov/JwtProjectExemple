namespace Jwt.WebUI.Entities
{
    public class User
    {
        public int Id { get; set; }
        public string? UserName { get; set; }
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime TokenCreated { get; set; }
        public DateTime TokenExpires { get; set; }
        public ERole Role { get; set; }
    }
}
