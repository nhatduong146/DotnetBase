namespace DotnetBase.Infrastructure.Mvc.Utilities
{
    using Microsoft.Extensions.DependencyInjection;
    using System.Collections.Generic;

    public class ServiceLocator
    {
        private static ServiceProvider _serviceProvider;

        /// <summary>
        /// Set the service provider
        /// </summary>
        /// <param name="serviceProvider"></param>
        public static void SetLocatorProvider(ServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        /// <summary>
        /// Get instance
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T GetInstance<T>() where T : class
        {
            return _serviceProvider.GetService<T>();
        }

        /// <summary>
        /// Get all instances
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static IEnumerable<T> GetInstances<T>() where T : class
        {
            return _serviceProvider.GetServices<T>();
        }
    }
}
