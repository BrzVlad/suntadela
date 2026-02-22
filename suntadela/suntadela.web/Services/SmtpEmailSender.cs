using System.Net;
using System.Net.Mail;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;

namespace suntadela.web.Services;

/// <summary>
///  Obtained from  WebApplication.CreateBuilder(args).GetSection("Smtp"):
///   1. appsettings.json
///   2. appsettings.{Environment}.json
///   3. User Secrets (only in dev, csproj has a UserSecretsId: dotnet user-secrets set "Smtp:Password" "pwd")
///   4. Environment variables (just set Smtp__Password)
///   5. Command-line args (dotnet run --Smtp:Password=mysecret)
/// </summary>
public class SmtpSettings
{
    public string Host { get; set; } = "";
    public int Port { get; set; } = 587;
    public string Username { get; set; } = "";
    public string Password { get; set; } = "";
    public string FromAddress { get; set; } = "";
    public string FromName { get; set; } = "";
    public bool EnableSsl { get; set; } = true;
}

public class SmtpEmailSender : IEmailSender
{
    private readonly SmtpSettings _settings;
    private readonly ILogger<SmtpEmailSender> _logger;

    public SmtpEmailSender(IOptions<SmtpSettings> options, ILogger<SmtpEmailSender> logger)
    {
        _settings = options.Value;
        _logger = logger;
    }

    public async Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        var message = new MailMessage
        {
            From = new MailAddress(_settings.FromAddress, _settings.FromName),
            Subject = subject,
            Body = htmlMessage,
            IsBodyHtml = true,
        };
        message.To.Add(email);

        using var client = new SmtpClient(_settings.Host, _settings.Port)
        {
            Credentials = new NetworkCredential(_settings.Username, _settings.Password),
            EnableSsl = _settings.EnableSsl,
        };

        try
        {
            await client.SendMailAsync(message);
            _logger.LogInformation("Email sent to {Email} with subject '{Subject}'.", email, subject);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send email to {Email} with subject '{Subject}'.", email, subject);
            throw;
        }
    }
}
