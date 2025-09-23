using My_books.Data.ViewModels;
using System.Net;

namespace My_books.Exceptions
{
    public class CustomExceptionMiddleware// exception custom baraye middle ware ha
    {
        private readonly RequestDelegate _next;

        #region [-Ctor-]
        public CustomExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }
        #endregion

      
        #region [-InvokeAsync-]
        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(httpContext, ex);
            }
        }
        #endregion

        #region [-HandleExceptionAsync-]
        private Task HandleExceptionAsync(HttpContext httpContext, Exception ex)
        {
            httpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            httpContext.Response.ContentType = "application/json";

            var response = new ErrorVM()
            {
                StatusCode = httpContext.Response.StatusCode,
                Message = "Internal Server Error from the custom middleware",
                Path = "path-goes-here"
            };

            return httpContext.Response.WriteAsync(response.ToString());
        }
        #endregion
    }
}
