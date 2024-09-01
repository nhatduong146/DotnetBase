using Microsoft.AspNetCore.Http;
using SendGrid.Helpers.Mail;
using System.Collections.Generic;

namespace DotnetBase.Infrastructure.Common.Models
{
    public class EmailMessageModel
    {
        public List<EmailAddress> Recipients { get; set; }
        public string Subject { get; set; }
        public string Content { get; set; }
        public List<IFormFile> Files { get; set; }


        public EmailMessageModel(List<EmailAddress> recipients, string subject, string content, List<IFormFile> fileNames)
        {
            Recipients = recipients;
            Subject = subject;
            Content = content;
            Files = fileNames;
        }
    }
}
