using DotnetBase.Infrastructure.Common.Models;
using MediatR;
using System.ComponentModel.DataAnnotations;

namespace DotnetBase.Application.Queries.Categories.Request
{
    public class CategoryCreateRequest : IRequest<ResponseModel>
    {
        [Required]
        [MaxLength(255)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(255)]
        public string Description { get; set; } = string.Empty;

        public List<Guid> ProductIds { get; set; }
    }
}
