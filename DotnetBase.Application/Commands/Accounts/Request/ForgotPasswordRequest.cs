using System.ComponentModel.DataAnnotations;
using MediatR;
using DotnetBase.Infrastructure.Common.Models;

namespace DotnetBase.Infrastructure.Mediators.Accounts.Request
{
    public class ForgotPasswordRequest : IRequest<ResponseModel>
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
