using DotnetBase.Infrastructure.Common.Models;
using MediatR;

namespace DotnetBase.Application.Queries.Categories.Request
{
    public class CategoryDeleteRequest : IRequest<ResponseModel>
    {
        public Guid Id { get; set; } = Guid.Empty;
    }
}
