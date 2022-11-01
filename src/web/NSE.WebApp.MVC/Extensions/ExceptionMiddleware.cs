using Microsoft.AspNetCore.Http;
using Polly.CircuitBreaker;
using Refit;
using System.Net;

namespace NSE.WebApp.MVC.Extensions
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (CustomHttpRequestException ex)
            {
                HandlerRquestExceptionAsync(httpContext, ex.StatusCode);
            }
            catch (BrokenCircuitException ex)
            {
                HandlerCircuitBreakerExceptionAsync(httpContext);
            }
            //catch (ValidationApiException ex)
            //{
            //    //exception refit
            //    HandlerRquestExceptionAsync(httpContext, ex.StatusCode);
            //}
            //catch (ApiException ex)
            //{
            //    //exception refit
            //    HandlerRquestExceptionAsync(httpContext, ex.StatusCode);
            //}
        }

        private static void HandlerRquestExceptionAsync(HttpContext httpContext, HttpStatusCode statusCode)
        {
            if (statusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                httpContext.Response.Redirect($"/login?ReturnUrl={httpContext.Request.Path}");
                return;
            }

            httpContext.Response.StatusCode = (int)statusCode;
        }

        private static void HandlerCircuitBreakerExceptionAsync(HttpContext httpContext)
        {
            httpContext.Response.Redirect("/sistema-indisponivel");
        }
    }
}
