using DotnetBase.Application.Common.Constants;
using DotnetBase.Application.Queries.Categories.Request;
using DotnetBase.Domain.Entities;
using DotnetBase.Domain.Entities.Contexts;
using DotnetBase.Infrastructure.Caching;
using DotnetBase.Infrastructure.Common.Models;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DotnetBase.Application.Categories.Handler
{
    public class CategoryEditHandler : IRequestHandler<CategoryEditRequest, ResponseModel>
    {
        private readonly AppDbContext _db;
        private readonly ICacheManager _cacheManager;

        /// <summary>
        ///   Initializes a new instance of the <see cref="CategoryEditHandler" /> class.
        /// </summary>
        /// <param name="db">The database context.</param>
        public CategoryEditHandler(
            AppDbContext db,
            ICacheManager cacheManager)
        {
            _db = db ?? throw new ArgumentNullException(nameof(db));
            _cacheManager = cacheManager ?? throw new ArgumentNullException(nameof(cacheManager));
        }

        public async Task<ResponseModel> Handle(CategoryEditRequest request, CancellationToken cancellationToken)
        {
            var category = await _db.Categories.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
            if (category == null)
            {
                return new ResponseModel()
                {
                    StatusCode = System.Net.HttpStatusCode.NotFound,
                    Message = AppMessageConstants.CATEGORY_NOT_FOUND
                };
            }

            request.Adapt(category);

            var removeProducts = _db.ProductInCategories.Where(x => !request.ProductIds.Contains(x.ProductId) && x.CategoryId == category.Id);
            var addProductIds = request.ProductIds.Where(productId => !removeProducts.Select(x => x.ProductId).Contains(productId)).ToList();

            _db.ProductInCategories.RemoveRange(removeProducts);

            addProductIds.ForEach(productId =>
            {
                _db.ProductInCategories.Add(new ProductInCategory()
                {
                    CategoryId = category.Id,
                    ProductId = productId
                });
            });

            await _db.SaveChangesAsync(cancellationToken);

            // remove cache after Category has been updated successfully
            var cacheKey = $"CATEGORY_{request.Id}";
            _cacheManager.Remove(cacheKey);

            return new ResponseModel()
            {
                StatusCode = System.Net.HttpStatusCode.OK,
                Message = AppMessageConstants.CATEGORY_UPDATED_SUCCESSFULLY
            };
        }
    }
}
