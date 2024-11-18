namespace GenericRepository.Extensions;

public static class ExcludePropertiesExtension
{
    public static IQueryable<T> ExcludeProperties<T>(this IQueryable<T> source, params string[] propertiesToExclude)
    {
        var parameter = Expression.Parameter(typeof(T), "x");
        var bindings = typeof(T)
            .GetProperties()
            .Where(p => !propertiesToExclude.Contains(p.Name))
            .Select(p => Expression.Bind(p, Expression.Property(parameter, p)))
            .ToList();

        var body = Expression.MemberInit(Expression.New(typeof(T)), bindings);
        var selector = Expression.Lambda<Func<T, T>>(body, parameter);

        return source.Select(selector);
    }
}