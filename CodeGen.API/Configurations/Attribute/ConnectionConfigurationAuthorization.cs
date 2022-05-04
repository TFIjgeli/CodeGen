using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Net;

namespace CodeGen.API.Configurations.Attribute
{
    public class ConnectionConfigurationAuthorization : System.Attribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext filterContext)
        {
            if (IsValidToken(filterContext.HttpContext.Request))
            {
                filterContext.HttpContext.Response.Headers.Add("AuthStatus", "Authorized");
                return;
            }
            else
            {
                filterContext.HttpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                filterContext.HttpContext.Response.HttpContext.Features.Get<IHttpResponseFeature>().ReasonPhrase = "Please Provide authToken";
                filterContext.Result = new JsonResult("Please Provide authToken")
                {
                    Value = new
                    {
                        Status = "Error",
                        Message = "Please Provide authToken"
                    },
                };
            }
        }

        private bool IsValidToken(HttpRequest request)
        {
            try
            {
                request.Headers.TryGetValue("X-LMS-Key", out var tracevalue);
                if (string.IsNullOrEmpty(tracevalue))
                    return false;

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
