using DotnetBase.Infrastructure.Common.Models;
using MediatR;

namespace DotnetBase.Application.Queries.Categories.Request
{
    public class CategoryPageListRequest : BaseRequestModel, IRequest<ResponseModel>
    {

    }
}
