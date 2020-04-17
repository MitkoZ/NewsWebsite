using Microsoft.Extensions.DependencyInjection;

namespace NewsWebsite.Utils
{
    public static class DependencyInjectionExtensions
    {
        public static void AddScoped<Interface1, Interface2, TImplementation>(this IServiceCollection services)
          where Interface1 : class
          where Interface2 : class
          where TImplementation : class, Interface1, Interface2
        {
            services.AddScoped<Interface1, TImplementation>();
            services.AddScoped<Interface2, TImplementation>(x => (TImplementation)x.GetService<Interface1>());
        }
    }
}
