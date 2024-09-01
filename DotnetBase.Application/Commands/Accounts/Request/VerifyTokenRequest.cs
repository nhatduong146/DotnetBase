using MediatR;
using Newtonsoft.Json;
using DotnetBase.Infrastructure.Common.Models;
using System.ComponentModel.DataAnnotations;

namespace DotnetBase.Infrastructure.Mediators.Accounts.Request
{
    public class VerifyTokenRequest : IRequest<ResponseModel>
    {
        [Required]
        [JsonProperty("access_token")]
        public string AccessToken { get; set; }
    }
}
