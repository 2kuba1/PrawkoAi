namespace Application.Common;

public interface ICachableRequest
{
    string CacheKey { get; }
    TimeSpan Expiration { get; }
}