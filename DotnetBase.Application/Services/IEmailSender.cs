using DotnetBase.Infrastructure.Common.Models;
using Microsoft.Extensions.Configuration;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace DotnetBase.Application.Services
{
    public interface IEmailSender
    {
        Task SendEmailAsync(EmailMessageModel emailMessageModel);
    }

    public class EmailSender : IEmailSender
    {
        private readonly IConfiguration _configuration;

        public EmailSender(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendEmailAsync(EmailMessageModel emailMessageModel)
        {
            var apiKey = _configuration["SendingEmail:SendGridApiKey"];
            var fromEmail = _configuration["SendingEmail:FromEmail"];
            var fromName = _configuration["SendingEmail:FromName"];
            var client = new SendGridClient(apiKey);
            var from = new EmailAddress(fromEmail, fromName);
            var msg = MailHelper.CreateSingleEmailToMultipleRecipients(from, emailMessageModel.Recipients, emailMessageModel.Subject, emailMessageModel.Content, emailMessageModel.Content);
            if (emailMessageModel.Files != null)
            {
                msg.Attachments = new List<Attachment>();
                foreach (var file in emailMessageModel.Files)
                {
                    if (file.Length > 0)
                    {
                        using (var ms = new MemoryStream())
                        {
                            file.CopyTo(ms);
                            var fileBytes = ms.ToArray();
                            var base64 = Convert.ToBase64String(fileBytes);
                            var att = new Attachment()
                            {
                                Filename = file.FileName,
                                Content = base64,
                                Type = file.ContentType
                            };
                            msg.Attachments.Add(att);
                        }
                    }
                }
            }
            await client.SendEmailAsync(msg);
        }
    }
}
