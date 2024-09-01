using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;

namespace DotnetBase.Infrastructure.Helpers
{
    public static class JwtHelper
    {
        public static object GenerateJwtToken(List<Claim> claims, IConfiguration configuration)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration.GetValue<string>("AppSettings:ThirdPartyRelationshipSecret")));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.Now.AddMinutes(Convert.ToDouble(configuration.GetValue<string>("AppSettings:TokenExpiredMinutes")));

            var token = new JwtSecurityToken(
                configuration.GetValue<string>("AppSettings:TokenIssuer"),
                configuration.GetValue<string>("AppSettings:TokenAudience"),
                claims,
                null,
                expires: expires,
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public static List<Claim> GenerateClaims(string claimType, List<string> claims)
        {
            var res = new List<Claim>();
            foreach (var claim in claims)
            {
                res.Add(new Claim(claimType, claim));
            }

            return res;
        }

        public static bool ValidateJwtToken(string token, IConfiguration configuration)
        {
            var tokenValidationParameters = new TokenValidationParameters()
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration.GetValue<string>("AppSettings:ThirdPartyRelationshipSecret"))),
                ValidAudience = configuration.GetValue<string>("AppSettings:TokenAudience"),
                ValidIssuer = configuration.GetValue<string>("AppSettings:TokenIssuer"),
                ClockSkew = TimeSpan.Zero
            };

            try
            {
                SecurityToken validatedToken;
                IPrincipal principal = new JwtSecurityTokenHandler().ValidateToken(token, tokenValidationParameters, out validatedToken);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
