namespace GenericRepository.Extensions;

public static class FilterByExtension
{
    public static IQueryable<T> FilterBy<T>(this IQueryable<T> collection, string filterBy) =>
        ParseFilterBy(filterBy).Aggregate(collection, ApplyFilterBy);

    private static IQueryable<T> ApplyFilterBy<T>(IQueryable<T> collection, FilterByInfo filterByInfo)
    {
        var type = typeof(T);
        var arg = Expression.Parameter(type, "x");
        var property = type.GetProperty(filterByInfo.PropertyName);
        var propertyAccess = Expression.MakeMemberAccess(arg, property!);
        var constant = Expression.Constant(Convert.ChangeType(filterByInfo.Value, property!.PropertyType));
        var body = GetExpressionBody(filterByInfo.Operator, propertyAccess, constant);
        var lambda = Expression.Lambda<Func<T, bool>>(body, arg);
        return collection.Where(lambda);
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
                typeof(string).GetMethod("Contains", [typeof(string)])!, constant),
            FilterOperator.StartsWith => Expression.Call(propertyAccess,
                typeof(string).GetMethod("StartsWith", [typeof(string)])!, constant),
            FilterOperator.EndsWith => Expression.Call(propertyAccess,
                typeof(string).GetMethod("EndsWith", [typeof(string)])!, constant),
            _ => throw new ArgumentOutOfRangeException(nameof(filterOperator), filterOperator, null)
        };
    }

    private static IEnumerable<FilterByInfo> ParseFilterBy(string filterBy)
    {
        if (string.IsNullOrEmpty(filterBy)) yield break;

        var items = filterBy.Split(',');
        foreach (var item in items)
        {
            var pair = item.Trim().Split(' ');
            if (pair.Length != 3)
                throw new ArgumentException(
                    $"Invalid FilterBy string '{item}'. Filter By Format: PropertyName Operator Value");

            var propertyName = pair[0];
            var filterOperator = Enum.Parse<FilterOperator>(pair[1], true);
            var value = ParseValue(pair[2]);

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
        if (float.TryParse(value, out var floatResult))
            return floatResult;
        if (decimal.TryParse(value, out var decimalResult))
            return decimalResult;
        if (DateTime.TryParse(value, out var dateTimeResult))
            return dateTimeResult;
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