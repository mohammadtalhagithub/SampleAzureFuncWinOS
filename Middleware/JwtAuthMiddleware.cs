using BeeSys.Utilities.Attributes;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.Functions.Worker.Middleware;
using System.Net;
using System.Reflection;

namespace BeeSys.Utilities.Middleware
{
    public class JwtAuthMiddleware : IFunctionsWorkerMiddleware
    {
        public async Task Invoke(FunctionContext context, FunctionExecutionDelegate next)
        {
            var request = await context.GetHttpRequestDataAsync();

            if (request == null)
            {
                await next(context);
                return;
            }

            var endpoint = context.FunctionDefinition.EntryPoint;
            var method = GetMethodInfo(endpoint);

            // If AllowAnonymous is present → skip auth
            if (method?.GetCustomAttribute<AllowAnonymousAttribute>() != null)
            {
                await next(context);
                return;
            }

            // If no Authorize attribute → skip auth
            if (method?.GetCustomAttribute<AuthorizeAttribute>() == null)
            {
                await next(context);
                return;
            }

            var httpContext = context.GetHttpContext();
            var result = await httpContext.AuthenticateAsync();

            if (!result.Succeeded)
            {
                var response = request.CreateResponse(HttpStatusCode.Unauthorized);
                await response.WriteStringAsync("Unauthorized");
                context.GetInvocationResult().Value = response;
                return;
            }

            httpContext.User = result.Principal;

            await next(context);
        }

        private MethodInfo? GetMethodInfo(string entryPoint)
        {
            var parts = entryPoint.Split('.');
            var typeName = string.Join('.', parts.Take(parts.Length - 1));
            var methodName = parts.Last();

            var type = Type.GetType(typeName);
            return type?.GetMethod(methodName);
        }
    }

#if false
    public class JwtAuthMiddleware : IFunctionsWorkerMiddleware
    {
        public async Task Invoke(FunctionContext context, FunctionExecutionDelegate next)
        {
            var request = await context.GetHttpRequestDataAsync();

            if (request == null)
            {
                await next(context);
                return;
            }

            var httpContext = context.GetHttpContext();

            var result = await httpContext.AuthenticateAsync();

            if (!result.Succeeded)
            {
                var response = request.CreateResponse(HttpStatusCode.Unauthorized);
                await response.WriteStringAsync("Unauthorized");
                context.GetInvocationResult().Value = response;
                return;
            }

            httpContext.User = result.Principal;

            await next(context);
        }
    } 
#endif
}
