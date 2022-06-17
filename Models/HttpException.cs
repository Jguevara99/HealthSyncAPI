using System.Net;

namespace ContosoPizza.Models;

[Serializable]
public class HttpException : Exception 
{
    public HttpStatusCode Code { get; set; }
    public object? Body { get; set; }

    public HttpException() { }

    public HttpException(string message): base(message) 
    {
        this.Code = HttpStatusCode.InternalServerError;
    }

    public HttpException(string message, HttpStatusCode code): base(message) 
    {
        this.Code = code;
    }

    public HttpException(string message, HttpStatusCode code, object? data): base(message) 
    {
        this.Code = code;
        this.Body = data;
    }
}