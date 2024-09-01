using MapsterMapper;
using DotnetBase.Application.Common.Constants;
using DotnetBase.Application.Queries.Categories.Request;
using DotnetBase.Application.Queries.Response;
using DotnetBase.Domain.Entities;
using DotnetBase.Domain.Entities.Contexts;
using DotnetBase.Infrastructure.Caching;
using DotnetBase.Infrastructure.Common.Models;
using DotnetBase.Infrastructure.Mvc.Utilities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using Mapster;

namespace DotnetBase.Application.Categories.Handler
{
    public class CategoryPatchHandler : IRequestHandler<CategoryPatchRequest, ResponseModel>
    {
        private readonly AppDbContext _db;
        private readonly ICacheManager _cacheManager;

        private readonly List<string> _patchAllowedFields = ReflectionUtilities.GetAllPropertyNamesOfType(typeof(CategoryEditRequest));

        /// <summary>
        ///   Initializes a new instance of the <see cref="CategoryPatchHandler" /> class.
        /// </summary>
        /// <param name="db">The database context.</param>
        public CategoryPatchHandler(
            AppDbContext db,
            ICacheManager cacheManager)
        {
            _db = db ?? throw new ArgumentNullException(nameof(db));
            _cacheManager = cacheManager ?? throw new ArgumentNullException(nameof(cacheManager));
        }

        public async Task<ResponseModel> Handle(CategoryPatchRequest request, CancellationToken cancellationToken)
        {
            var packageObj = request.CategoryPatchModel;

            if (!ValidState(packageObj))
            {
                return new ResponseModel()
                {
                    StatusCode = System.Net.HttpStatusCode.BadRequest,
                    Message = AppMessageConstants.CATEGORY_PATCH_UPDATE_NO_FIELD
                };
            }

            var category = await _db.Categories.FirstOrDefaultAsync(x => x.Id == request.Id);
            if (category == null)
            {
                return new ResponseModel()
                {
                    StatusCode = System.Net.HttpStatusCode.NotFound,
                    Message = AppMessageConstants.CATEGORY_NOT_FOUND
                };
            }

            foreach (var obj in packageObj)
            {
                var key = _patchAllowedFields.SingleOrDefault(_ => _.Equals(obj.Key, StringComparison.InvariantCultureIgnoreCase));
                if (key == null)
                {
                    return new ResponseModel()
                    {
                        StatusCode = System.Net.HttpStatusCode.Forbidden,
                        Message = AppMessageConstants.CATEGORY_PATCH_UPDATE_NOT_MATCH_FIELD
                    };
                }

                var jValue = obj.Value;
                if (jValue == null) continue;

                var propertyName = key;
                var targetType = typeof(Category);
                var myPropInfo = targetType.GetProperty(propertyName);

                dynamic newValue = "";
                if (myPropInfo.PropertyType == typeof(decimal) || myPropInfo.PropertyType == typeof(decimal?))
                    newValue = jValue.Value<decimal?>();
                else if (myPropInfo.PropertyType == typeof(int) || myPropInfo.PropertyType == typeof(int?))
                    newValue = jValue.Value<int?>();
                else if (myPropInfo.PropertyType == typeof(string))
                    newValue = jValue.Value<string>()?.Trim();

                if (targetType == typeof(Category))
                    myPropInfo.SetValue(category, newValue);
            }

            _db.Categories.Update(category);
            await _db.SaveChangesAsync(cancellationToken);

            // remove cache after Category has been patched successfully
            var cacheKey = $"CATEGORY_{request.Id}";
            _cacheManager.Remove(cacheKey);

            return new ResponseModel()
            {
                StatusCode = System.Net.HttpStatusCode.OK,
                Message = AppMessageConstants.CATEGORY_UPDATED_SUCCESSFULLY,
                Data = category.Adapt<CategoryResponse>()
            };
        }

        private bool ValidState(JObject jObj)
        {
            return jObj != null && jObj.Properties().Count() > 0;
        }
    }
}
