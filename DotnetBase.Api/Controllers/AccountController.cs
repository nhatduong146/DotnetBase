using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using DotnetBase.Infrastructure.Mvc.Filters;
using System.Threading;
using System.Threading.Tasks;
using DotnetBase.Infrastructure.Mediators.Accounts.Request;

namespace DotnetBase.Api.Controllers
{
    [Route("api/account")]
    [Consumes("application/json")]
    [Produces("application/json")]

    public class AccountController : ControllerBase
    {
        private readonly IMediator _mediator;

        /// <summary>
        ///   Initializes a new instance of the <see cref="AccountController" /> class.
        /// </summary>
        /// <param name="mediator">The mediator.</param>
        public AccountController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<dynamic> Login([FromBody] UserLoginRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            return new BaseActionResult(result);
        }

        [AllowAnonymous]
        [HttpPost("verify-token")]
        public async Task<dynamic> VerifyToken([FromBody] VerifyTokenRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            return new BaseActionResult(result);
        }

        [AllowAnonymous]
        [HttpPost("forgot-password")]
        public async Task<dynamic> ForgotPassword([FromBody] ForgotPasswordRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            return new BaseActionResult(result);
        }

        [AllowAnonymous]
        [HttpPost("reset-password")]
        public async Task<dynamic> ResetPassword([FromBody] ResetPasswordRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            return new BaseActionResult(result);
        }
    }
}
