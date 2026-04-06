using System.Net;
using Domain.Shared;

namespace Domain.Exceptions;

public class QuestionNotFoundException(string message) : BaseException(message)
{
    public override HttpStatusCode StatusCode { get; } =  HttpStatusCode.NotFound;
}