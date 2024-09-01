using MapsterMapper;
using DotnetBase.Application.Queries.Categories.Request;
using DotnetBase.Application.Queries.Response;
using DotnetBase.Domain.Entities.Contexts;
using DotnetBase.Infrastructure.Common.Models;
using DotnetBase.Infrastructure.Helpers;
using MediatR;

namespace DotnetBase.Application.Queries.Handler
{
    public class CategoryPageListHandler : IRequestHandler<CategoryPageListRequest, ResponseModel>
    {
        private readonly AppDbContext _db;

        /// <summary>
        ///   Initializes a new instance of the <see cref="CategoryPageListHandler" /> class.
        /// </summary>
        /// <param name="db">The database context.</param>
        public CategoryPageListHandler(
            AppDbContext db)
        {
            _db = db ?? throw new ArgumentNullException(nameof(db));
        }

        public async Task<ResponseModel> Handle(CategoryPageListRequest request, CancellationToken cancellationToken)
        {
            var list = _db.Categories.Where(
                x => string.IsNullOrEmpty(request.SearchTerm)
                    || x.Name.Contains(request.SearchTerm))
                .Select(x => new CategoryResponse()
                {
                    Id = x.Id,
                    Name = x.Name,
                    Desciption = x.Description
                });

            var sortAndPaginationModel = new SortAndPaginationModel()
            {
                DefaultSortingProperty = "Name",
                SortingProperties = request.SortingProperties,
                PageIndex = request.PageIndex,
                PageSize = request.PageSize
            };

            var pageList = QueryHelper.SortAndPaginationDynamic(list, sortAndPaginationModel);

            return new ResponseModel()
            {
                StatusCode = System.Net.HttpStatusCode.OK,
                Data = pageList
            };
        }
    }
}
