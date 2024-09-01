namespace DotnetBase.Infrastructure.Mediators.Accounts.Handler
{
    using Common.Models;
    using DotnetBase.Application.Common.Constants;
    using DotnetBase.Domain.Entities;
    using DotnetBase.Domain.Entities.Contexts;
    using MediatR;
    using Microsoft.AspNetCore.Identity;
    using Request;
    using System;
    using System.Linq;
    using System.Net;
    using System.Threading;
    using System.Threading.Tasks;

    public class ResetPasswordHandler : IRequestHandler<ResetPasswordRequest, ResponseModel>
    {
        private readonly AppDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;

        public ResetPasswordHandler(AppDbContext dbContext, UserManager<ApplicationUser> userManager)
        {
            _db = dbContext;
            _userManager = userManager;
        }

        public async Task<ResponseModel> Handle(ResetPasswordRequest request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
                return new ResponseModel
                {
                    StatusCode = HttpStatusCode.NotFound,
                    Message = AppMessageConstants.USER_NOT_FOUND
                };

            var resetPasswordToken = _db.ResetPasswordTokens.FirstOrDefault(x => x.ResetPasswordCode == request.ResetPasswordCode && x.ExpiredTime > DateTime.Now);
            if (resetPasswordToken == null)
                return new ResponseModel
                {
                    StatusCode = HttpStatusCode.NotFound,
                    Message = "Reset password code is wrong or reset password code is expired"
                };

            var resetPassResult = await _userManager.ResetPasswordAsync(user, resetPasswordToken.Token, request.Password);
            if (!resetPassResult.Succeeded)
            {
                return new ResponseModel
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    Message = resetPassResult.Errors.FirstOrDefault().Description
                };
            }
            else
            {
                return new ResponseModel
                {
                    StatusCode = HttpStatusCode.OK,
                    Data = "The password has been reset successfully"
                };
            }
        }
    }
}
