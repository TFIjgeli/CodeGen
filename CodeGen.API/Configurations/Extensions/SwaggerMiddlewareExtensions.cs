using Microsoft.AspNetCore.Builder;

namespace CodeGen.API.Configurations.Extensions
{
    public static class SwaggerMiddlewareExtensions
    {
        public static void ConfigureSwaggerHandler(this IApplicationBuilder app)
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
                c.RoutePrefix = string.Empty;
            });
        }
    }
}
