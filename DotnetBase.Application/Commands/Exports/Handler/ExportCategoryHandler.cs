using DotnetBase.Application.Exports.Request;
using DotnetBase.Application.Queries.Response;
using DotnetBase.Domain.Entities.Contexts;
using DotnetBase.Infrastructure.Common.Models;
using DotnetBase.Infrastructure.Helpers;
using DotnetBase.Infrastructure.Mvc.Utilities;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DotnetBase.Application.Exports.Handler
{
    public class ExportCategoryHandler : IRequestHandler<ExportCategoryRequest, ResponseModel>
    {
        private readonly AppDbContext _db;

        /// <summary>
        ///   Initializes a new instance of the <see cref="ExportCategoryHandler" /> class.
        /// </summary>
        /// <param name="db">The database context.</param>
        public ExportCategoryHandler(
            AppDbContext db)
        {
            _db = db ?? throw new ArgumentNullException(nameof(db));
        }

        public async Task<ResponseModel> Handle(ExportCategoryRequest request, CancellationToken cancellationToken)
        {
            var list = _db.Categories.AsNoTracking()
                .Where(x => string.IsNullOrEmpty(request.SearchTerm) || x.Name.Contains(request.SearchTerm))
                .ProjectToType<CategoryResponse>();

            List<CategoryResponse> sources;
            if (request.ExportType == ExportType.All)
            {
                sources = await list.ToListAsync(cancellationToken: cancellationToken);
            }
            else
            {
                var sortAndPaginationModel = new SortAndPaginationModel()
                {
                    DefaultSortingProperty = "Name",
                    SortingProperties = request.SortingProperties,
                    PageIndex = request.PageIndex,
                    PageSize = request.PageSize
                };

                var pageList = QueryHelper.SortAndPaginationDynamic<CategoryResponse>(list, sortAndPaginationModel);
                sources = await pageList.Sources.ToListAsync(cancellationToken: cancellationToken);
            }

            var stream = ExportExcelUtilities.ExportExcel<CategoryResponse>(sources);
            return new ResponseModel()
            {
                StatusCode = System.Net.HttpStatusCode.OK,
                Data = stream
            };
        }
    }
}
