namespace GenericRepository.EntityFramework;

public interface IRepository<T> where T : Entity, new()
{
    IQueryable<T> Queryable(Expression<Func<T, bool>>? filter = null, string[]? includes = null);

    Task<T?> GetAsync(Expression<Func<T, bool>>? filter = null, string[]? includes = null,
        CancellationToken cancellationToken = default);

    Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>>? filter = null, string[]? includes = null,
        CancellationToken cancellationToken = default);

    IQueryable<T> GetPaginatedQueryable(Expression<Func<T, bool>>? filter = null, string[]? includes = null,
        Expression<Func<T, bool>>? search = null, string? orderBy = null,
        int pageNumber = 1, int pageSize = 10, CancellationToken cancellationToken = default);

    Task<PaginatedResponse<T>> GetPaginatedAsync(Expression<Func<T, bool>>? filter = null, string[]? includes = null,
        Expression<Func<T, bool>>? search = null, string? orderBy = null,
        int pageNumber = 1, int pageSize = 10, CancellationToken cancellationToken = default);

    Task<T> AddAsync(T entity, CancellationToken cancellationToken = default);

    Task<IEnumerable<T>> AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default);

    Task<T> UpdateAsync(T entity, CancellationToken cancellationToken = default);

    Task<IEnumerable<T>> UpdateRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default);

    Task<T?> DeleteAsync(Guid id, CancellationToken cancellationToken = default);

    Task DeleteRangeAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken = default);
}