using CodeGen.Domain.Common;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using System;
using System.Net;
using System.Threading.Tasks;

namespace CodeGen.API.Configurations.Extensions
{
    public static class ExceptionMiddlewareExtensions
    {
        public static void ConfigureExceptionHandler(this IApplicationBuilder app)
        {
            app = MaintainCorsHeadersOnError(app);

            app.UseExceptionHandler(appError =>
            {
                appError.Run(async context =>
                {
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    context.Response.ContentType = "application/json";

                    var contextFeature = context.Features.Get<IExceptionHandlerFeature>();
                    if (contextFeature != null)
                    {
                        /// (For Next)
                        /// Add code here
                        /// ex: Add Error logging
                        //ErrorHelpers.CreateErrorLogs(contextFeature.Error.ToString(), contextFeature.Error.Message);

                        await context.Response.WriteAsync(new ModelStateError
                        {
                            Title = "Error",
                            ErrorMessage = contextFeature.Error.ToString()
                        }
                        .ToString());
                    }
                });
            });
        }

        public static IApplicationBuilder MaintainCorsHeadersOnError(this IApplicationBuilder builder)
        {
            return builder.Use(async (httpContext, next) =>
            {
                var corsHeaders = new HeaderDictionary();
                foreach (var pair in httpContext.Response.Headers)
                {
                    if (!pair.Key.StartsWith("access-control-", StringComparison.InvariantCultureIgnoreCase)) { continue; }
                    corsHeaders[pair.Key] = pair.Value;
                }

                httpContext.Response.OnStarting(o =>
                {
                    var ctx = (HttpContext)o;
                    var headers = ctx.Response.Headers;
                    foreach (var pair in corsHeaders)
                    {
                        if (headers.ContainsKey(pair.Key)) { continue; }
                        headers.Add(pair.Key, pair.Value);
                    }
                    return Task.CompletedTask;
                }, httpContext);

                await next();
            });
        }

    }
}
