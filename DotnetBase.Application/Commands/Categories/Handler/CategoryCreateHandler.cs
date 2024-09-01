using DotnetBase.Application.Common.Constants;
using DotnetBase.Application.Queries.Categories.Request;
using DotnetBase.Domain.Entities;
using DotnetBase.Domain.Entities.Contexts;
using DotnetBase.Infrastructure.Common.Models;
using Mapster;
using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace DotnetBase.Application.Queries.Handler
{
    public class CategoryCreateHandler : IRequestHandler<CategoryCreateRequest, ResponseModel>
    {
        private readonly AppDbContext _db;
        private readonly IHttpContextAccessor _contextAccessor;

        /// <summary>
        ///   Initializes a new instance of the <see cref="CategoryCreateHandler" /> class.
        /// </summary>
        public CategoryCreateHandler(
            AppDbContext db,
            IHttpContextAccessor contextAccessor)
        {
            _db = db ?? throw new ArgumentNullException(nameof(db));
            _contextAccessor = contextAccessor ?? throw new ArgumentNullException(nameof(contextAccessor));
        }

        public async Task<ResponseModel> Handle(CategoryCreateRequest request, CancellationToken cancellationToken)
        {
            var category = request.Adapt<Category>();

            // current user id logged in
            var userId = _contextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

            // assign current user id logged in to created by
            category.CreatedBy = new Guid(userId);

            request.ProductIds.ForEach(productId =>
            {
                _db.ProductInCategories.Add(new ProductInCategory()
                {
                    CategoryId = category.Id,
                    ProductId = productId
                });
            });

            _db.Categories.Add(category);

            await _db.SaveChangesAsync(cancellationToken);

            return new ResponseModel()
            {
                StatusCode = System.Net.HttpStatusCode.OK,
                Message = AppMessageConstants.CATEGORY_CREATED_SUCCESSFULLY
            };
        }
    }
}
