using DotnetBase.Application.Queries.Categories.Request;
using DotnetBase.Application.Queries.Response;
using DotnetBase.Domain.Entities;
using Mapster;

namespace DotnetBase.Application
{
    public class MapsterRegister : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            #region User Register

            config.ForType<Category, CategoryResponse>()
                .Map(dest => dest.Desciption, src => src.Description);
            config.ForType<CategoryCreateRequest, Category>();
            config.ForType<CategoryEditRequest, Category>()
                .Ignore(x => x.Id);

            #endregion
        }
    }
}
