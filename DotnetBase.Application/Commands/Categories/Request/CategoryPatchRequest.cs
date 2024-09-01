using DotnetBase.Infrastructure.Common.Models;
using MediatR;
using Newtonsoft.Json.Linq;
using System.Text.Json.Serialization;

namespace DotnetBase.Application.Queries.Categories.Request
{
    public class CategoryPatchRequest : IRequest<ResponseModel>
    {
        [JsonIgnore]
        public Guid Id { get; set; }

        public JObject CategoryPatchModel { get; set; }
    }
}
