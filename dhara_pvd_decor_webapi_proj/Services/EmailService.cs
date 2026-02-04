using dhara_pvd_decor_webapi_proj.Services;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;

public class EmailService : IEmailService
{
    private readonly EmailSettings _settings;

    public EmailService(IOptions<EmailSettings> settings)
    {
        _settings = settings.Value;
    }

    public async Task SendWelcomeEmail(string toEmail, string userName, string plainPassword)
    {
        var email = new MimeMessage();
        email.From.Add(new MailboxAddress(_settings.SenderName, _settings.SenderEmail));
        email.To.Add(MailboxAddress.Parse(toEmail));
        email.Subject = "🎉 Welcome to Dhara PVD Decor";

        email.Body = new TextPart(MimeKit.Text.TextFormat.Html)
        {
            Text = $@"
                <h2>Welcome, {userName}! 👋</h2>
                <p>Your account has been successfully created.</p>
                <p><b>Login Details:</b></p>
                <ul>
                    <li><b>Username:</b> {userName}</li>
                    <li><b>Password:</b> {plainPassword}</li>
                </ul>
                <p>Please change your password after first login.</p>
                <br/>
                <p>Warm regards,<br/>
                <b>Dhara PVD Decor Team</b></p>
            "
        };

        using var smtp = new SmtpClient();
        await smtp.ConnectAsync(_settings.SmtpServer, _settings.Port, SecureSocketOptions.StartTls);
        await smtp.AuthenticateAsync(_settings.Username, _settings.Password);
        await smtp.SendAsync(email);
        await smtp.DisconnectAsync(true);
    }

    public class EmailSettings
    {
        public string SmtpServer { get; set; } = "";
        public int Port { get; set; }
        public string SenderName { get; set; } = "";
        public string SenderEmail { get; set; } = "";
        public string Username { get; set; } = "";
        public string Password { get; set; } = "";
    }
}
