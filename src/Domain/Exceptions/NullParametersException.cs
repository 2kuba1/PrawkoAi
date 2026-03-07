using System.Net;
using Domain.Shared;

namespace Domain.Exceptions;

public class NullParametersException(string message) : BaseException(message)
{
    public override HttpStatusCode StatusCode { get; }
}