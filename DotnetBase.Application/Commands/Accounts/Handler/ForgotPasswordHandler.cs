namespace DotnetBase.Infrastructure.Mediators.Accounts.Handler
{
    using Common.Models;
    using DotnetBase.Application.Common.Constants;
    using DotnetBase.Application.Services;
    using DotnetBase.Domain.Entities;
    using DotnetBase.Domain.Entities.Contexts;
    using Helpers;
    using MediatR;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.Extensions.Configuration;
    using Request;
    using SendGrid.Helpers.Mail;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Threading;
    using System.Threading.Tasks;

    public class ForgotPasswordHandler : IRequestHandler<ForgotPasswordRequest, ResponseModel>
    {
        private readonly AppDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly IEmailSender _emailSender;

        public ForgotPasswordHandler(AppDbContext dbContext,
            UserManager<ApplicationUser> userManager,
            IConfiguration configuration,
            IEmailSender emailSender)
        {
            _db = dbContext;
            _userManager = userManager;
            _configuration = configuration;
            _emailSender = emailSender;
        }

        public async Task<ResponseModel> Handle(ForgotPasswordRequest request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
                return new ResponseModel
                {
                    StatusCode = HttpStatusCode.NotFound,
                    Message = AppMessageConstants.USER_NOT_FOUND
                };


            var emailExpired = _db.ResetPasswordTokens.FirstOrDefault(x => x.Email == request.Email && x.ExpiredTime > DateTime.Now);
            var resetPasswordCode = emailExpired != null ? emailExpired.ResetPasswordCode : StringHelper.GenerateOtpNumber(6);

            var siteUrl = _configuration["SendingEmail:SiteUrl"];
            var resetPasswordPath = siteUrl + "reset-password/otp=" + resetPasswordCode;

            var templatePath = _configuration["SendingEmail:ResetPasswordPath"];
            var template = await System.IO.File.ReadAllTextAsync(templatePath, cancellationToken);

            var body = template.Replace("{EMAIL}", request.Email).Replace("{ResetPasswordPath}", resetPasswordPath);
            var fromEmail = _configuration["SendingEmail:FromEmail"];

            var emailMessageModel = new EmailMessageModel(new List<EmailAddress>()
                {
                    new EmailAddress(request.Email, fromEmail)
                }, "Forgot Password", body, null);

            if (emailExpired != null)
            {
                await _emailSender.SendEmailAsync(emailMessageModel);
                return new ResponseModel
                {
                    StatusCode = HttpStatusCode.NotFound,
                    Message = "An email has been sent again, please check email box."
                };
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            var otpToken = new ResetPasswordToken()
            {
                Email = request.Email,
                ResetPasswordCode = resetPasswordCode,
                Token = token,
                ExpiredTime = DateTime.Now.AddHours(Convert.ToDouble(_configuration.GetSection("AppSettings")["OTPTokenExpiredHours"]))
            };

            _db.Add(otpToken);
            await _db.SaveChangesAsync(cancellationToken);

            await _emailSender.SendEmailAsync(emailMessageModel);

            return new ResponseModel
            {
                StatusCode = HttpStatusCode.OK,
                Data = "Email has been sent"
            };
        }
    }
}
