namespace GenericRepository.Models.Request;

public abstract class PaginatedRequest
{
    public int Page { get; init; }
    public int PageSize { get; init; }
    public string? OrderBy { get; init; }
    public string? SearchTerm { get; init; }
}