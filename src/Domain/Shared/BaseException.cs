using System.Net;

namespace Domain.Shared;

public abstract class BaseException(string message) : Exception(message)
{
    public abstract HttpStatusCode StatusCode { get; }
}