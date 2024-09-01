namespace DotnetBase.Infrastructure.Mediators.Accounts.Handler
{
    using System;
    using System.Collections.Generic;
    using System.IdentityModel.Tokens.Jwt;
    using System.Linq;
    using System.Net;
    using System.Security.Claims;
    using System.Threading;
    using System.Threading.Tasks;
    using MediatR;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.Extensions.Configuration;
    using DotnetBase.Domain.Entities;
    using Helpers;
    using Common.Models;
    using Request;

    public class UserLoginHandler : IRequestHandler<UserLoginRequest, ResponseModel>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IConfiguration _configuration;

        public UserLoginHandler(UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IConfiguration configuration)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
        }

        public async Task<ResponseModel> Handle(UserLoginRequest request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByEmailAsync(request.Email)
                   ?? await _userManager.FindByNameAsync(request.Email);

            var passwordIsCorrect = await _userManager.CheckPasswordAsync(user, request.Password);
            if (!passwordIsCorrect)
            {
                return new ResponseModel
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    Message = "User or Password is invalid"
                };
            }
            else
            {
                var roles = await _userManager.GetRolesAsync(user);
                var claims = new List<Claim>
                {
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Name, user.Name)
                };
                claims.AddRange(JwtHelper.GenerateClaims(ClaimTypes.Role, roles.ToList()));

                var claimsPrincipal = await _signInManager.CreateUserPrincipalAsync(user);
                if (claimsPrincipal?.Identity is not ClaimsIdentity claimsIdentity)
                    return new ResponseModel
                    {
                        StatusCode = HttpStatusCode.OK,
                        Data = new
                        {
                            access_token = JwtHelper.GenerateJwtToken(claims, _configuration)
                        }
                    };
                claimsIdentity.AddClaims(claims);
                await _signInManager.SignInWithClaimsAsync(user,
                    true,
                    claimsIdentity.Claims);

                return new ResponseModel
                {
                    StatusCode = HttpStatusCode.OK,
                    Data = new
                    {
                        access_token = JwtHelper.GenerateJwtToken(claims, _configuration)
                    }
                };
            }
        }
    }
}
