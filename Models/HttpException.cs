using System.Net;

namespace ContosoPizza.Models;

[Serializable]
public class HttpException : Exception 
{
    public int StatusCode { get; set; }
    public object? Body { get; set; }

    public HttpException() { }

    public HttpException(string message): base(message) 
    {
        StatusCode = (int)HttpStatusCode.InternalServerError;
    }

    public HttpException(string message, int statusCode): base(message) 
    {
        StatusCode = statusCode;
    }

    public HttpException(string message, HttpStatusCode statusCode): base(message) 
    {
        StatusCode = (int)statusCode;
    }


    public HttpException(string message, int statusCode, object? body): base(message) 
    {
        StatusCode = statusCode;
        Body = body;
    }
    
    public HttpException(string message, HttpStatusCode statusCode, object? body): base(message) 
    {
        StatusCode = (int)statusCode;
        Body = body;
    }
}