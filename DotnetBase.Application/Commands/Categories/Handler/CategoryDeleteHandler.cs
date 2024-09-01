using DotnetBase.Application.Common.Constants;
using DotnetBase.Application.Queries.Categories.Request;
using DotnetBase.Domain.Entities.Contexts;
using DotnetBase.Infrastructure.Caching;
using DotnetBase.Infrastructure.Common.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DotnetBase.Application.Queries.Categories.Handler
{
    public class CategoryDeleteHandler : IRequestHandler<CategoryDeleteRequest, ResponseModel>
    {
        private readonly AppDbContext _db;
        private readonly ICacheManager _cacheManager;

        /// <summary>
        ///   Initializes a new instance of the <see cref="CategoryDeleteHandler" /> class.
        /// </summary>
        /// <param name="db">The database context.</param>
        /// <param name="cacheManager">The cache manager.</param>
        public CategoryDeleteHandler(AppDbContext db, ICacheManager cacheManager)
        {
            _db = db ?? throw new ArgumentNullException(nameof(db));
            _cacheManager = cacheManager ?? throw new ArgumentNullException(nameof(cacheManager));
        }

        public async Task<ResponseModel> Handle(CategoryDeleteRequest request, CancellationToken cancellationToken)
        {
            var category = await _db.Categories.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
            if (category == null)
            {
                return new ResponseModel()
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    Message = AppMessageConstants.CATEGORY_DELETED_SUCCESSFULLY
                };
            }

            _db.Categories.Remove(category);
            await _db.SaveChangesAsync(cancellationToken);

            // remove cache after Category has been deleted successfully
            var cacheKey = $"CATEGORY_{request.Id}";
            _cacheManager.Remove(cacheKey);


            return new ResponseModel()
            {
                StatusCode = System.Net.HttpStatusCode.OK,
                Message = AppMessageConstants.CATEGORY_DELETED_SUCCESSFULLY
            };
        }
    }
}
