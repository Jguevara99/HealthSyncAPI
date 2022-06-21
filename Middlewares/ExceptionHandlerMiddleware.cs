using ContosoPizza.Models;

namespace ContosoPizza.Middlewares;

public class ExceptionHandlerMiddleware
{
    private readonly RequestDelegate _next;

    public ExceptionHandlerMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next.Invoke(context);
        }
        catch (Exception error)
        {
            switch (error)
            {
                case HttpException e:
                    context.Response.StatusCode = e.StatusCode;
                    await context.Response
                                 .WriteAsJsonAsync<BaseHttpResponse>(
                                     new BaseHttpResponse(e.Message, e.StatusCode, false, e.Body)
                                 );
                    break;

                default:
                    context.Response.StatusCode = 500;
                    await context.Response
                                 .WriteAsJsonAsync<BaseHttpResponse>(
                                     new BaseHttpResponse(error.Message, 
                                                          System.Net.HttpStatusCode.InternalServerError, 
                                                          false, error.Data)
                                    );
                    break;
            }
        }
    }
}