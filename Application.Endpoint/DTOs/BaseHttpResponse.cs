using System.ComponentModel;
using System.Net;

namespace Application.Endpoint.DTOs;

public class BaseHttpResponse<T>
{
    public int Code { get; set; }
    public string? Message { get; set; }
    public bool Success { get; set; }

    [DefaultValue(null)]
    public T? Data { get; set; }

    public BaseHttpResponse(string message, HttpStatusCode code, bool success)
    {
        Message = message;
        Code = (int)code;
        Success = success;
    }

    public BaseHttpResponse(string message, int code, bool success)
    {
        Message = message;
        Code = code;
        Success = success;
    }

    public BaseHttpResponse(string message, HttpStatusCode code, bool success, T? data)
    {
        Message = message;
        Code = (int)code;
        Success = success;
        Data = data;
    }

    public BaseHttpResponse(string message, int code, bool success, T? data)
    {
        Message = message;
        Code = code;
        Success = success;
        Data = data;
    }
}

[DisplayName("HttpResponse")]
public class BaseHttpResponse : BaseHttpResponse<object?>
{
    public BaseHttpResponse(string message, HttpStatusCode code, bool success) : base(message, code, success) { }
    public BaseHttpResponse(string message, int code, bool success) : base(message, code, success) { }

    public BaseHttpResponse(string message, HttpStatusCode code, bool success, object? data) : base(message, code, success, data) { }
    public BaseHttpResponse(string message, int code, bool success, object? data) : base(message, code, success, data) { }
}

public class Ok<T> : BaseHttpResponse<T>
{
    public Ok(string message) : base(message, HttpStatusCode.OK, true) { }

    public Ok(string message, T? data) : base(message, HttpStatusCode.OK, true, data) { }
}

public class Ok : Ok<object>
{
    public Ok(string message) : base(message) { }

    public Ok(string message, object? data) : base(message, data) { }
}

public class NotFound<T> : BaseHttpResponse<T>
{
    public NotFound(string message) : base(message, HttpStatusCode.NotFound, false) { }

    public NotFound(string message, T? data) : base(message, HttpStatusCode.NotFound, false, data) { }
}

public class NotFound : NotFound<object?>
{
    public NotFound(string message) : base(message) { }

    public NotFound(string message, object? data) : base(message, data) { }
}

public class BadRequest<T> : BaseHttpResponse<T>
{
    public BadRequest(string message) : base(message, HttpStatusCode.BadRequest, false) { }

    public BadRequest(string message, T? data) : base(message, HttpStatusCode.BadRequest, false, data) { }
}

public class BadRequest : BaseHttpResponse<object>
{
    public BadRequest(string message) : base(message, HttpStatusCode.BadRequest, false) { }
}

public class Created<T> : BaseHttpResponse<T>
{
    public Created(string message) : base(message, HttpStatusCode.Created, true) { }

    public Created(string message, T? data) : base(message, HttpStatusCode.Created, true, data) { }
}

public class Unauthorized : BaseHttpResponse<object>
{
    public Unauthorized(string message) : base(message, HttpStatusCode.Unauthorized, false) { }
}

public class ModelStateError
{
    public bool IsModelStateError { get => true; }
    public IEnumerable<ModelPropertyError> errors { get; set; } = new List<ModelPropertyError>();
}

public class ModelPropertyError
{
    public string Key { get; set; } = string.Empty;
    public IEnumerable<string> errors { get; set; } = new List<string>();
}
