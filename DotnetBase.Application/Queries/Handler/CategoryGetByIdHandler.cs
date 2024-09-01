using MapsterMapper;
using DotnetBase.Application.Common.Constants;
using DotnetBase.Application.Queries.Request;
using DotnetBase.Application.Queries.Response;
using DotnetBase.Domain.Entities.Contexts;
using DotnetBase.Infrastructure.Caching;
using DotnetBase.Infrastructure.Common.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Mapster;

namespace DotnetBase.Application.Queries.Handler
{
    public class CategoryGetByIdHandler : IRequestHandler<CategoryGetByIdRequest, ResponseModel>
    {
        private readonly AppDbContext _db;
        private readonly ICacheManager _cacheManager;

        /// <summary>
        ///   Initializes a new instance of the <see cref="CategoryGetByIdHandler" /> class.
        /// </summary>
        /// <param name="db">The database context.</param>
        /// <param name="cacheManager">The cache manager.</param>
        public CategoryGetByIdHandler(
            AppDbContext db,
            ICacheManager cacheManager)
        {
            _db = db ?? throw new ArgumentNullException(nameof(db));
            _cacheManager = cacheManager ?? throw new ArgumentNullException(nameof(cacheManager));
        }

        public async Task<ResponseModel> Handle(CategoryGetByIdRequest request, CancellationToken cancellationToken)
        {
            var cacheKey = $"CATEGORY_{request.Id}";

            var category = await _cacheManager.GetAndSetAsync(cacheKey, 1, () =>
            {
                return _db.Categories
                .FirstOrDefaultAsync(x => x.Id == request.Id);
            });

            if (category == null)
            {
                return new ResponseModel()
                {
                    StatusCode = System.Net.HttpStatusCode.NotFound,
                    Message = AppMessageConstants.CATEGORY_NOT_FOUND
                };
            }
            return new ResponseModel()
            {
                StatusCode = System.Net.HttpStatusCode.OK,
                Data = category.Adapt<CategoryResponse>()
            };
        }
    }
}
