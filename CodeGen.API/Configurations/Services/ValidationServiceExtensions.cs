using CodeGen.Application.Configurations.Validations;
using Microsoft.Extensions.DependencyInjection;

namespace CodeGen.API.Configurations.Services
{
    public static class ValidationServiceExtensions
    {
        public static void AddValidators(this IServiceCollection services)
        {
            services.Scan(scan => scan
              .FromAssemblyOf<IValidationHandler>()
                .AddClasses(classes => classes.AssignableTo<IValidationHandler>())
                  .AsImplementedInterfaces()
                  .WithTransientLifetime());
        }
    }
}
