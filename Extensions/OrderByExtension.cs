namespace GenericRepository.Extensions;

public static class OrderByExtension
{
    public static IEnumerable<T> OrderBy<T>(this IEnumerable<T> enumerable, string orderBy) =>
        enumerable.AsQueryable().OrderBy(orderBy).AsEnumerable();

    public static IQueryable<T> OrderBy<T>(this IQueryable<T> collection, string orderBy) =>
        ParseOrderBy(orderBy).Aggregate(collection, ApplyOrderBy);

    private static IQueryable<T> ApplyOrderBy<T>(IQueryable<T>? collection, OrderByInfo orderByInfo)
    {
        var type = typeof(T);
        var arg = Expression.Parameter(type, "x");
        Expression propertyAccess = GetNestedPropertyExpression(arg, orderByInfo.PropertyName);

        var delegateType = typeof(Func<,>).MakeGenericType(typeof(T), propertyAccess.Type);
        var lambda = Expression.Lambda(delegateType, propertyAccess, arg);
        string methodName;

        if (!orderByInfo.Initial && collection is IOrderedQueryable<T>)
        {
            methodName = orderByInfo.Direction == SortDirection.Ascending ? "ThenBy" : "ThenByDescending";
        }
        else
        {
            methodName = orderByInfo.Direction == SortDirection.Ascending ? "OrderBy" : "OrderByDescending";
        }

        return (IOrderedQueryable<T>)typeof(Queryable).GetMethods().Single(
                method => method.Name == methodName
                          && method.IsGenericMethodDefinition
                          && method.GetGenericArguments().Length == 2
                          && method.GetParameters().Length == 2)
            .MakeGenericMethod(typeof(T), propertyAccess.Type)
            .Invoke(null, [collection, lambda])!;
    }

    private static Expression GetNestedPropertyExpression(Expression parameter, string propertyName)
    {
        var properties = propertyName.Split('.');
        Expression propertyAccess = parameter;

        foreach (var property in properties)
        {
            var propertyInfo = propertyAccess.Type.GetProperty(property);
            if (propertyInfo == null)
            {
                throw new ArgumentException($"Property '{property}' not found on type '{propertyAccess.Type.Name}'");
            }

            if (propertyAccess.Type.IsClass || Nullable.GetUnderlyingType(propertyAccess.Type) != null)
            {
                var nullCheck = Expression.Equal(propertyAccess, Expression.Constant(null, propertyAccess.Type));
                var defaultValue = Expression.Constant(
                    propertyInfo.PropertyType.IsValueType ?
                        Activator.CreateInstance(propertyInfo.PropertyType) :
                        null,
                    propertyInfo.PropertyType);
                var propertyValue = Expression.Property(propertyAccess, propertyInfo);
                propertyAccess = Expression.Condition(nullCheck, defaultValue, propertyValue);
            }
            else
            {
                propertyAccess = Expression.Property(propertyAccess, propertyInfo);
            }
        }

        return propertyAccess;
    }

    private static IEnumerable<OrderByInfo> ParseOrderBy(string orderBy)
    {
        if (string.IsNullOrEmpty(orderBy)) yield break;

        var items = orderBy.Split(',');
        var initial = true;

        foreach (var item in items)
        {
            var pair = item.Trim().Split(' ');

            if (pair.Length > 2)
                throw new ArgumentException(
                    $"Invalid OrderBy string '{item}'. Order By Format: Property, Property2 ASC, Property2 DESC"
                );

            var prop = pair[0].Trim();

            if (string.IsNullOrEmpty(prop))
                throw new ArgumentException(
                    "Invalid Property. Order By Format: Property, Property2 ASC, Property2 DESC");

            var dir = SortDirection.Ascending;

            if (pair.Length == 2)
                dir = "desc".Equals(pair[1].Trim(), StringComparison.OrdinalIgnoreCase)
                    ? SortDirection.Descending
                    : SortDirection.Ascending;

            yield return new OrderByInfo() { PropertyName = prop, Direction = dir, Initial = initial };

            initial = false;
        }
    }

    private class OrderByInfo
    {
        public string PropertyName { get; set; } = null!;
        public SortDirection Direction { get; set; }
        public bool Initial { get; set; }
    }

    private enum SortDirection
    {
        Ascending = 0,
        Descending = 1
    }
}