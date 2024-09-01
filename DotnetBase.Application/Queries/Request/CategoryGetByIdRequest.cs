using DotnetBase.Infrastructure.Common.Models;
using MediatR;

namespace DotnetBase.Application.Queries.Request
{
    public class CategoryGetByIdRequest : IRequest<ResponseModel>
    {
        public Guid Id { get; set; } = Guid.Empty;
    }
}
