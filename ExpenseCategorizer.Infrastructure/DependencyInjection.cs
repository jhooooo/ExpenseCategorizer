// ExpenseCategorizer.Infrastructure/DependencyInjection.cs
using Microsoft.Extensions.DependencyInjection;
using ExpenseCategorizer.Application; // IAIService

namespace ExpenseCategorizer.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
        {
            services.AddScoped<IAIService, OpenAIService>(); 
            return services;
        }
    }
}