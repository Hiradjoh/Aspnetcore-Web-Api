using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http.Features;
using My_books.Data.ViewModels;
using System.Net;

namespace My_books.Exceptions.MiddleWares
{
    public static class ExceptionMiddlewareExtensions// tarif middle ware ha 
    {
        #region [-Configure-BuiltIn-Exception-Handler-]
        public static void ConfigureBuildInExceptionHandler(this IApplicationBuilder app)
        {
            app.UseExceptionHandler(appError =>
            {
                appError.Run(async context =>
                {
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    context.Response.ContentType = "application/json";
                    var contextFeature = context.Features.Get<IExceptionHandlerFeature>();
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
