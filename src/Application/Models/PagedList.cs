namespace Application.Models;

public record PagedList<T>(
    List<T> Items,
    int PageNumber,
    int TotalCount,
    int TotalPages)
{
    public bool HasNextPage => PageNumber < TotalPages;
}