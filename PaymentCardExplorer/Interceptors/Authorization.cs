using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Utilities.Binders;

namespace PaymentCardExplorer.Interceptors
{
    public class Authorization
    {
        private readonly AuthTokens _authTokens;
        private readonly RequestDelegate _next;

        public Authorization(RequestDelegate next, IOptions<AuthTokens> authTokens)
        {
            _authTokens = authTokens.Value;
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            string _authToken = _authTokens.BearerToken;
            string reqAuthToken = context.Request.Headers.FirstOrDefault(header => header.Key == "Authorization").Value.FirstOrDefault();

            if (IsAuthorized(_authToken, reqAuthToken, context))
            {

                await _next.Invoke(context);
                return;
            }
            context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;

            await HttpResponseWritingExtensions.WriteAsync(context.Response, "Invalid Authorization Token");
        }

        private bool IsAuthorized(string authToken, string reqAuthToken, HttpContext context)
        {
            //Check if path is base API url
            var baseUrl = context.Request.Path.ToString().ToLower().Split("/");
            if (string.IsNullOrEmpty(baseUrl[1]) || baseUrl[1] == "swagger" || (baseUrl[1] == "api" && baseUrl?.Length == 2)) return true;
            else
            {
                var bearerToken = reqAuthToken?.Split("Bearer ");
                if(bearerToken?.Length != 2)
                {
                    return false;
                }
                else
                {
                    return authToken == bearerToken[1];
                }
            }
            
        }
    }
}
