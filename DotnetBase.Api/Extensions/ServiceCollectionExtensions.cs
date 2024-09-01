using DotnetBase.Infrastructure.Mvc.Utilities;
using Mapster;
using Microsoft.Extensions.DependencyInjection;

namespace DotnetBase.Api.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void AddMapster(this IServiceCollection services)
        {
            var typeAdapterConfig = TypeAdapterConfig.GlobalSettings;
            typeAdapterConfig.Scan(ReflectionUtilities.GetAssemblies());
        }
    }
}
