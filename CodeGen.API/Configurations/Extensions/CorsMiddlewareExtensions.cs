using Microsoft.AspNetCore.Builder;

namespace CodeGen.API.Configurations.Extensions
{
    public static class CorsMiddlewareExtensions
    {
        public static void ConfigureCorsHandler(this IApplicationBuilder app)
        {
            app.UseCors(x => x
                   .AllowAnyMethod()
                   .AllowAnyHeader()
                   .SetIsOriginAllowed(origin => true) // allow any origin
                   .AllowCredentials()); // allow credentials

        }
    }
}
