namespace GenericRepository.Models.Response;

public class PaginatedResponse<T>
{
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
    public List<T> Data { get; set; } = [];
}