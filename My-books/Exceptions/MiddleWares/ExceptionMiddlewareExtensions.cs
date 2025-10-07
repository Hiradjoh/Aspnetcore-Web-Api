using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http.Features;
using My_books.Data.ViewModels;
using System.Net;

namespace My_books.Exceptions.MiddleWares
{
    public static class ExceptionMiddlewareExtensions// tarif middle ware ha 
    /// Provides extension methods to configure exception handling middlewares in the application.
    /// Includes both the built-in ASP.NET Core exception handler and a custom exception middleware.
    {
        #region [-Configure-BuiltIn-Exception-Handler-]
        //daroon khodesh ex dare 
        public static void ConfigureBuildInExceptionHandler(this IApplicationBuilder app) // exception that occurs in the pipeline will be caught by this middleware .

        {
            app.UseExceptionHandler(appError =>// Adds a middleware to the pipeline that will catch exceptions, log them, and re-execute
                                               //     the request in an alternate pipeline. The request will not be re-executed if
                                               //     the response has already started.
                                               //
                                               // Parameters:
                                               //   app:
                                               //
                                               //   options:
            {
                appError.Run(async context =>
                {
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    context.Response.ContentType = "application/json";
                    var contextFeature = context.Features.Get<IExceptionHandlerFeature>();//With these lines, you can get the error details and the request path .
                    var contextRequest = context.Features.Get<IHttpRequestFeature>();

                    if (contextFeature != null)
                    {
                        await context.Response.WriteAsync(new ErrorVM()
                        {
                            StatusCode = context.Response.StatusCode,
                            Message = contextFeature.Error.Message,
                            Path = contextRequest.Path
                        }.ToString());
                    }
                });
            });
        }
        #endregion

        #region [-Configure-Custom-Exception-Handler-]
        public static void ConfigureCustomExceptionHandler(this IApplicationBuilder app)
        {
            app.UseMiddleware<CustomExceptionMiddleware>();
        }
        #endregion
    }
}
