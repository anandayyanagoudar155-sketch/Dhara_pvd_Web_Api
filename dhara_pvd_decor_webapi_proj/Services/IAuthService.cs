using dhara_pvd_decor_webapi_proj.Controllers;

namespace dhara_pvd_decor_webapi_proj.Services
{
    public interface IAuthService
    {
        Task<int> RegisterUser(AuthController.RegisterRequest request);

        Task<AuthController.User?> ValidateLogin( AuthController.UservalidateLoginRequest request);

        Task<AuthLoginResponse?> SaveLogin( AuthController.UsersaveLoginRequest request );

        Task<int> Logout(string jti);

        HealthResponse GetHealth(int port);
    }

    public class AuthLoginResponse
    {
        public string Token { get; set; } = "";
        public long UserId { get; set; }
        public string UserName { get; set; } = "";
        public string UserRole { get; set; } = "";
        public long CompId { get; set; }
        public string CompName { get; set; } = "";
        public long FinYearId { get; set; }
        public string FinName { get; set; } = "";
        public DateTime? YearStart { get; set; }
        public DateTime? YearEnd { get; set; }
    }

    public class HealthResponse
    {
        public string Status { get; set; } = "Healthy";
        public int Port { get; set; }
        public string Server { get; set; } = "";
        public DateTime Time { get; set; }
    }
}
