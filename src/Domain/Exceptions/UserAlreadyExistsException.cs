using System.Net;
using Domain.Shared;

namespace Domain.Exceptions;

public class UserAlreadyExistsException(string message) : BaseException(message)
{
    public override HttpStatusCode StatusCode { get; } =  HttpStatusCode.BadRequest;
}