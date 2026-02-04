namespace dhara_pvd_decor_webapi_proj.Services
{
    public interface IEmailService
    {
        Task SendWelcomeEmail(string toEmail, string userName, string plainPassword);
    }
}
