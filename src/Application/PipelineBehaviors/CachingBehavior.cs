using System.Text.Json;
using Application.Common;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;

namespace Application.PipelineBehaviors;

internal sealed class CachingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : ICachableRequest
{
    private readonly IDistributedCache _cache;

    public CachingBehavior(IDistributedCache cache)
    {
        _cache = cache;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var cachedData = await _cache.GetStringAsync(request.CacheKey, cancellationToken);
        if (cachedData is not null)
        {
            return JsonSerializer.Deserialize<TResponse>(cachedData)!;
        }

        var response = await next(cancellationToken);

        var cacheOptions = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = request.Expiration
        };
        
        await _cache.SetStringAsync(request.CacheKey, JsonSerializer.Serialize(response), cacheOptions, cancellationToken);

        return response;
    }
}