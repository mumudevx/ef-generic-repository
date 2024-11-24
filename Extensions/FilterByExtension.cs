namespace GenericRepository.Extensions;

public static class FilterByExtension
{
    public static IQueryable<T> FilterBy<T>(this IQueryable<T> collection, params string[] filterBy) =>
        filterBy.SelectMany(ParseFilterBy).Aggregate(collection, ApplyFilterBy);

    private static IQueryable<T> ApplyFilterBy<T>(IQueryable<T> collection, FilterByInfo filterByInfo)
    {
        var type = typeof(T);
        var arg = Expression.Parameter(type, "x");

        var propertyAccess = filterByInfo.PropertyName.Split('.').Length > 2
            ? GetDeptLevelPropertyExpression(arg, filterByInfo.PropertyName)
            : GetNestedPropertyExpression(arg, filterByInfo.PropertyName);

        if (filterByInfo.Value is IEnumerable<int> intList)
        {
            var constant = Expression.Constant(intList);
            var containsMethod = typeof(List<int>).GetMethod("Contains", new[] { typeof(int) });
            var body = Expression.Call(constant, containsMethod!, propertyAccess);

            var lambda = Expression.Lambda<Func<T, bool>>(body, arg);
            return collection.Where(lambda);
        }
        else
        {
            var constant = Expression.Constant(Convert.ChangeType(filterByInfo.Value, propertyAccess.Type));
            var body = GetExpressionBody(filterByInfo.Operator, propertyAccess, constant);
            var lambda = Expression.Lambda<Func<T, bool>>(body, arg);
            return collection.Where(lambda);
        }
    }

    private static Expression GetNestedPropertyExpression(Expression parameter, string propertyName)
    {
        var properties = propertyName.Split('.');
        var propertyAccess = parameter;
        var type = parameter.Type;

        foreach (var property in properties)
        {
            var propertyInfo = type.GetProperty(property);
            if (propertyInfo == null)
            {
                throw new ArgumentException($"Property '{property}' not found on type '{type.Name}'");
            }

            propertyAccess = Expression.Property(propertyAccess, propertyInfo);
            type = propertyInfo.PropertyType;
        }

        return propertyAccess;
    }

    private static Expression GetDeptLevelPropertyExpression(Expression parameter, string propertyName)
    {
        var properties = propertyName.Split('.');
        var propertyAccess = parameter;

        foreach (var property in properties)
        {
            var propertyInfo = propertyAccess.Type.GetProperty(property);
            if (propertyInfo == null)
            {
                throw new ArgumentException($"Property '{property}' not found on type '{propertyAccess.Type.Name}'");
            }

            var nullCheck = Expression.Equal(propertyAccess, Expression.Constant(null, propertyAccess.Type));
            propertyAccess = Expression.Property(propertyAccess, propertyInfo);
            propertyAccess = Expression.Condition(
                nullCheck,
                Expression.Constant(null, propertyInfo.PropertyType),
                propertyAccess
            );
        }

        return propertyAccess;
    }

    private static Expression GetExpressionBody(FilterOperator filterOperator, Expression propertyAccess,
        ConstantExpression constant)
    {
        return filterOperator switch
        {
            FilterOperator.Equal => Expression.Equal(propertyAccess, constant),
            FilterOperator.NotEqual => Expression.NotEqual(propertyAccess, constant),
            FilterOperator.GreaterThan => Expression.GreaterThan(propertyAccess, constant),
            FilterOperator.GreaterThanOrEqual => Expression.GreaterThanOrEqual(propertyAccess, constant),
            FilterOperator.LessThan => Expression.LessThan(propertyAccess, constant),
            FilterOperator.LessThanOrEqual => Expression.LessThanOrEqual(propertyAccess, constant),
            FilterOperator.Contains => Expression.Call(propertyAccess,
                typeof(string).GetMethod("Contains", new[] { typeof(string) })!, constant),
            FilterOperator.StartsWith => Expression.Call(propertyAccess,
                typeof(string).GetMethod("StartsWith", new[] { typeof(string) })!, constant),
            FilterOperator.EndsWith => Expression.Call(propertyAccess,
                typeof(string).GetMethod("EndsWith", new[] { typeof(string) })!, constant),
            _ => throw new ArgumentOutOfRangeException(nameof(filterOperator), filterOperator, null)
        };
    }

    private static IEnumerable<FilterByInfo> ParseFilterBy(string filterBy)
    {
        var filters = filterBy.Split(new[] { " AND ", " OR " }, StringSplitOptions.None);
        foreach (var filter in filters)
        {
            var parts = filter.Split(new[] { ' ' }, 3);
            if (parts.Length != 3)
            {
                throw new ArgumentException(
                    $"Invalid FilterBy string '{filterBy}'. Filter By Format: PropertyName Operator Value");
            }

            var propertyName = parts[0];
            var filterOperator = Enum.Parse<FilterOperator>(parts[1], true);
            var value = ParseValue(parts[2]);

            yield return new FilterByInfo
            {
                PropertyName = propertyName,
                Operator = filterOperator,
                Value = value
            };
        }
    }

    private static object ParseValue(string value)
    {
        if (bool.TryParse(value, out var boolResult))
            return boolResult;
        if (int.TryParse(value, out var intResult))
            return intResult;
        if (float.TryParse(value, CultureInfo.InvariantCulture, out var floatResult))
            return floatResult;
        if (decimal.TryParse(value, out var decimalResult))
            return decimalResult;
        if (DateTime.TryParse(value, out var dateTimeResult))
            return dateTimeResult;

        if (value.Contains('['))
        {
            var trimmedValue = value.Trim('[', ']');
            var parts = trimmedValue.Split(',');
            var intList = parts.Select(int.Parse).ToList();
            return intList;
        }

        return value;
    }
}

internal class FilterByInfo
{
    public string PropertyName { get; init; } = string.Empty;
    public object Value { get; init; } = string.Empty;
    public FilterOperator Operator { get; init; }
}

public enum FilterOperator
{
    Equal,
    NotEqual,
    GreaterThan,
    GreaterThanOrEqual,
    LessThan,
    LessThanOrEqual,
    Contains,
    StartsWith,
    EndsWith
}