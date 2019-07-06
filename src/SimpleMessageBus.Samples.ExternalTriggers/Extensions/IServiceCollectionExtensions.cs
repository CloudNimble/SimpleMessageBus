using SimpleMessageBus.Samples.ExternalTriggers;

namespace Microsoft.Extensions.DependencyInjection
{

    /// <summary>
    /// 
    /// </summary>
    public static class IServiceCollectionExtensions
    {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddTimerDependencies(this IServiceCollection services)
        {
            //services.AddSingleton<SampleTimers, SampleTimers>();
            return services;
        }

    }

}