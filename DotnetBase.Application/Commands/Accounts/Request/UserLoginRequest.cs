namespace DotnetBase.Infrastructure.Mediators.Accounts.Request
{
    using System.ComponentModel.DataAnnotations;
    using MediatR;
    using Common.Models;

    public class UserLoginRequest : IRequest<ResponseModel>
    {
        [Required]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
