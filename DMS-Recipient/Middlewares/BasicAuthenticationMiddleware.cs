using DMS_recipient.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System;
using System.Text;
using System.Threading.Tasks;

namespace DMS_recipient.Middlewares
{
    // You may need to install the Microsoft.AspNetCore.Http.Abstractions package into your project
    public class BasicAuthenticationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IOptionsMonitor<Configs> _config;

        public BasicAuthenticationMiddleware(RequestDelegate next,
            IOptionsMonitor<Configs> optionsMonitor)
        {
            _next = next;
            _config = optionsMonitor;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            string authHeader = httpContext.Request.Headers["Authorization"];

            if (authHeader != null && authHeader.StartsWith("Basic"))
            {
                string encodeUsernameAndPassword = authHeader.Substring("Basic ".Length).Trim();
                Encoding encoding = Encoding.GetEncoding("utf-8");

                string usernameAndPassword = encoding.GetString(Convert.FromBase64String(encodeUsernameAndPassword));

                if (usernameAndPassword.Contains(':')) { 
                    string _username = usernameAndPassword.Substring(0, usernameAndPassword.IndexOf(':'));
                    string _password = usernameAndPassword.Substring(usernameAndPassword.IndexOf(':') + 1);

                    if (_config.CurrentValue.Username.ToLower() == _username.ToLower() &&
                        _config.CurrentValue.Password.Trim() == _password)
                    {
                        await _next.Invoke(httpContext);
                    }
                    else {
                        httpContext.Response.StatusCode = 401;
                        return;
                    }                    
                }                
            }
            else {
                httpContext.Response.StatusCode = 401;
                return;
            }
        }
    }

    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class BasicAuthenticationMiddlewareExtensions
    {
        public static IApplicationBuilder UseBasicAuthenticationMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<BasicAuthenticationMiddleware>();
        }
    }
}
