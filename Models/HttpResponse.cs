using System.Net;

namespace ContosoPizza.Models;

public class BaseHttpResponse {
    public HttpStatusCode Code { get; set; }
    public string? Message { get; set; }
    public bool Success { get; set; }
    public object? Data { get; set; }

    public BaseHttpResponse(string message, HttpStatusCode code, bool success)
    {
        Message = message;
        Code = code;
        Success = success;
    }
    
    public BaseHttpResponse(string message, HttpStatusCode code, bool success, object? data)
    {
        Message = message;
        Code = code;
        Success = success;
        Data = data;
    }
}

public class Ok : BaseHttpResponse
{
    public Ok(string message): base(message, HttpStatusCode.OK, true) { }

    public Ok(string message, object? data): base(message, HttpStatusCode.OK, true, data) { }
}

public class NotFound : BaseHttpResponse
{

    public NotFound(string message): base(message, HttpStatusCode.NotFound, false) { }

    public NotFound(string message, object? data): base(message, HttpStatusCode.NotFound, false, data) { }
}

public class BadRequest : BaseHttpResponse
{
    public BadRequest(string message): base(message, HttpStatusCode.BadRequest, false) { }

    public BadRequest(string message, object? data): base(message, HttpStatusCode.BadRequest, false, data) { }
}


public class Created : BaseHttpResponse
{
    public Created(string message): base(message, HttpStatusCode.Created, true) { }

    public Created(string message, object? data): base(message, HttpStatusCode.Created, true, data) { }
}