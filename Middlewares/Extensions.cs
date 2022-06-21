namespace ContosoPizza.Middlewares.Extensions;

public static class MiddlewareExtensions
{
    public static void UseCustomExceptionHandlerMiddleware(this IApplicationBuilder app)
    {
        app.UseMiddleware<ExceptionHandlerMiddleware>();
    }
}