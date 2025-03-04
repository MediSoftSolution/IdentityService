namespace IdentityService.Models.Login
{
    public class LoginRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }


        public string CacheKey => "Login";

        public double CacheTime => 60;
    }
}
