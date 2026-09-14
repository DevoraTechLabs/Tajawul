namespace Tajawul.Models.ViewModels.Auth
{
    public class SuccessSigninDto
    {
        public string Email { get; set; } = null!;

        public IList<string> Role { get; set; } = null!;

        public string Token { get; set; } = null!;

        public string RefreshToken { get; set; } = null!;
    }
}
