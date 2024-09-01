using DotnetBase.Infrastructure.Common.Models;
using MediatR;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace DotnetBase.Application.Queries.Categories.Request
{
    public class CategoryEditRequest : IRequest<ResponseModel>
    {
        [JsonIgnore]
        public Guid Id { get; set; } = Guid.Empty;

        [Required]
        [MaxLength(255)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(255)]
        public string Description { get; set; } = string.Empty;

        public List<Guid> ProductIds { get; set; }
    }
}
