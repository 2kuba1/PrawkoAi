namespace Application.Contracts.Services;

public interface IAiService
{
    Task<string?> GenerateContentAsync(string prompt, CancellationToken cancellationToken);

    IAsyncEnumerable<string> StreamContentAsync(
        string prompt,
        CancellationToken cancellationToken);
}