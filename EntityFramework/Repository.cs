namespace GenericRepository.EntityFramework;

public class Repository<T>(DbContext context) : IRepository<T> where T : Entity, new()
{
    private readonly DbSet<T> _dbSet = context.Set<T>();

    public IQueryable<T> Queryable(Expression<Func<T, bool>>? filter = null, string[]? includes = null)
    {
        var query = _dbSet.AsQueryable();

        if (filter != null)
            query = query.Where(filter);

        if (includes != null)
            query = includes.Aggregate(query, (current, include) => current.Include(include));

        return query;
    }

    public async Task<T?> GetAsync(Expression<Func<T, bool>>? filter = null, string[]? includes = null,
        CancellationToken cancellationToken = default)
    {
        var query = _dbSet.AsQueryable();

        if (filter != null)
            query = query.Where(filter);

        if (includes != null)
            query = includes.Aggregate(query, (current, include) => current.Include(include));

        return await query.FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>>? filter = null, string[]? includes = null,
        CancellationToken cancellationToken = default)
    {
        var query = _dbSet.AsQueryable();

        if (filter != null)
            query = query.Where(filter);

        if (includes != null)
            query = includes.Aggregate(query, (current, include) => current.Include(include));

        return await query.ToListAsync(cancellationToken);
    }

    public IQueryable<T> GetPaginatedQueryable(Expression<Func<T, bool>>? filter = null,
        string[]? includes = null, Expression<Func<T, bool>>? search = null, string? orderBy = null,
        int pageNumber = 1, int pageSize = 10, CancellationToken cancellationToken = default)
    {
        var query = _dbSet.AsQueryable();

        if (filter != null)
            query = query.Where(filter);

        if (includes != null)
            query = includes.Aggregate(query, (current, include) => current.Include(include));

        if (search != null)
            query = query.Where(search);

        if (orderBy != null)
            query = query.OrderBy(orderBy);

        var items = query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize);

        return items;
    }

    public async Task<PaginatedResponse<T>> GetPaginatedAsync(Expression<Func<T, bool>>? filter = null,
        string[]? includes = null, Expression<Func<T, bool>>? search = null,
        string? orderBy = null, int pageNumber = 1, int pageSize = 10, CancellationToken cancellationToken = default)
    {
        var query = _dbSet.AsQueryable();

        if (includes != null)
            query = includes.Aggregate(query, (current, include) => current.Include(include));

        if (search != null)
            query = query.Where(search);

        if (orderBy != null)
            query = query.OrderBy(orderBy);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken: cancellationToken);

        return new PaginatedResponse<T>
        {
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalCount = totalCount,
            Data = items
        };
    }

    public async Task<T> AddAsync(T entity, CancellationToken cancellationToken = default)
    {
        await _dbSet.AddAsync(entity, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        return entity;
    }

    public async Task<IEnumerable<T>> AddRangeAsync(IEnumerable<T> entities,
        CancellationToken cancellationToken = default)
    {
        var entityList = entities.ToList();

        await _dbSet.AddRangeAsync(entityList, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        return entityList;
    }

    public async Task<T> UpdateAsync(T entity, CancellationToken cancellationToken = default)
    {
        var existingEntity = await _dbSet.FindAsync([entity.Id], cancellationToken);

        if (existingEntity == null)
            throw new InvalidOperationException("Entity not found.");

        _dbSet.Update(entity);
        await context.SaveChangesAsync(cancellationToken);

        return existingEntity;
    }

    public async Task<IEnumerable<T>> UpdateRangeAsync(IEnumerable<T> entities,
        CancellationToken cancellationToken = default)
    {
        var entityList = entities.ToList();

        foreach (var entity in entityList)
        {
            var existingEntity = await _dbSet.FindAsync([entity.Id], cancellationToken);

            if (existingEntity == null)
                continue;

            context.Entry(existingEntity).CurrentValues.SetValues(entity);
        }

        await context.SaveChangesAsync(cancellationToken);

        return entityList;
    }

    public async Task<T?> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _dbSet.FindAsync([id], cancellationToken);

        if (entity == null)
            return null;

        _dbSet.Remove(entity);

        return entity;
    }

    public async Task DeleteRangeAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken = default)
    {
        var entities = await _dbSet.Where(e => ids.Contains(e.Id)).ToListAsync(cancellationToken);

        if (entities.Count != 0)
        {
            _dbSet.RemoveRange(entities);
            await context.SaveChangesAsync(cancellationToken);
        }
    }
}