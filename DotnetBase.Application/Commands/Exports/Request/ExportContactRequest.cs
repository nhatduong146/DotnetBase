
using MediatR;
using DotnetBase.Infrastructure.Common.Models;

namespace DotnetBase.Application.Exports.Request
{
    public class ExportCategoryRequest : BaseRequestModel, IRequest<ResponseModel>
    {
        public ExportType ExportType { get; set; } = ExportType.All;
    }

    public enum ExportType
    {
        All = 1,
        CurrentPage = 2
    }
}
