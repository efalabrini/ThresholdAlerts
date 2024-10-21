using Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using Core.Services;
using Microsoft.Extensions.Logging;
using System.Security.Principal;
using System.Runtime.InteropServices.Marshalling;
using Microsoft.Extensions.Options;

namespace Infrastructure.Services;

public class EmailNotificationService : INotificationService
{
    private readonly SmtpClient _smtpClient;
    private readonly string _mailFrom;
    private readonly ILogger<AlertService> _logger;
    private readonly EmailNotificationServiceOptions _options;

    public async Task NotifyAsync(string msg, string to)
    {
        await SendAsync(to, msg, "", "");
    }

    public EmailNotificationService(IOptions<EmailNotificationServiceOptions> options, ILogger<AlertService> logger)
    {
        _options = options.Value;
        _smtpClient = new SmtpClient(_options.Host, _options.Port);

        NetworkCredential networkCredential = new NetworkCredential();
        networkCredential.UserName = _options.UserName;
        networkCredential.Password = _options.Password;

        _smtpClient.Credentials = networkCredential;
        _smtpClient.EnableSsl = true;

        _mailFrom = _options.MailFrom;
        _logger = logger;


    }

    public async Task SendAsync(string to, string subject, string body, string attachments)
    {

        try
        {

            var message = new MailMessage
            {
                From = new MailAddress(_mailFrom),
                Subject = subject,
                Body = body
            };

            var mails = to.Split(";");
            foreach (string mail in mails)
            {
                message.To.Add(new MailAddress(mail));
            }

            if (!string.IsNullOrEmpty(attachments))
                message.Attachments.Add(new Attachment(attachments));

            _logger.LogInformation("Starting email notification to {to}", to);
            await _smtpClient.SendMailAsync(message);
            _logger.LogInformation("Finishing email notification to {to}", to);
        }
        catch
        (Exception ex)
        {
            _logger.LogError("Error {errorMessage}",ex.Message);
        }
        

    }

}


public class EmailNotificationServiceOptions
{
    public const string EmailNotificationService = "EmailNotificationService";
    public string Host { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;

    public int Port { get; set; }

    public string Password { get; set; } = string.Empty;

    public string MailFrom { get; set; } = string.Empty;


}