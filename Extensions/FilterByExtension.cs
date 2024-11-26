namespace GenericRepository.Extensions;

public static class FilterByExtension
{
    public static IQueryable<T> FilterBy<T>(this IQueryable<T> collection, params string[] filterBy)
    {
        return filterBy.Aggregate(collection, (current, filter) =>
        {
            var conditions = ParseFilterBy(filter).ToList();

            if (conditions.Count == 0)
                return current;

            var parameter = Expression.Parameter(typeof(T), "x");

            var orGroups = conditions.Split(c => !c.IsAndCondition);

            // Combine AND conditions within each OR group, then combine OR groups
            var combinedExpression = orGroups
                .Select(group => group
                    .Select(f => BuildFilterExpression(parameter, f))
                    .Aggregate(Expression.AndAlso))
                .Aggregate(Expression.OrElse);

            var lambda = Expression.Lambda<Func<T, bool>>(combinedExpression, parameter);
            return current.Where(lambda);
        });
    }

    private static Expression BuildFilterExpression(ParameterExpression parameter, FilterByInfo filterByInfo)
    {
        var propertyAccess = filterByInfo.PropertyName.Split('.').Length > 2
            ? GetDeptLevelPropertyExpression(parameter, filterByInfo.PropertyName)
            : GetNestedPropertyExpression(parameter, filterByInfo.PropertyName);

        if (filterByInfo.Value is IEnumerable<int> intList)
        {
            var constant = Expression.Constant(intList);
            var containsMethod = typeof(List<int>).GetMethod("Contains", [typeof(int)]);

            return Expression.Call(constant,
                containsMethod ?? throw new InvalidOperationException(),
                propertyAccess);
        }
        else
        {
            var constant = Expression.Constant(Convert.ChangeType(filterByInfo.Value, propertyAccess.Type));
            return GetExpressionBody(filterByInfo.Operator, propertyAccess, constant);
        }
    }

    private static IEnumerable<IEnumerable<T>> Split<T>(this IEnumerable<T> source, Func<T, bool> predicate)
    {
        var group = new List<T>();

        foreach (var item in source)
        {
            if (predicate(item) && group.Any())
            {
                yield return group;
                group = [];
            }

            group.Add(item);
        }

        if (group.Any())
            yield return group;
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
                typeof(string).GetMethod("Contains", [typeof(string)])
                ?? throw new InvalidOperationException(), constant),
            FilterOperator.StartsWith => Expression.Call(propertyAccess,
                typeof(string).GetMethod("StartsWith", [typeof(string)])
                ?? throw new InvalidOperationException(), constant),
            FilterOperator.EndsWith => Expression.Call(propertyAccess,
                typeof(string).GetMethod("EndsWith", [typeof(string)])
                ?? throw new InvalidOperationException(), constant),
            _ => throw new ArgumentOutOfRangeException(nameof(filterOperator), filterOperator, null)
        };
    }

    private static IEnumerable<FilterByInfo> ParseFilterBy(string filterBy)
    {
        var orGroups = filterBy.Split([" OR "], StringSplitOptions.None);

        foreach (var orGroup in orGroups)
        {
            // For each OR group, create a list of AND conditions
            var andConditions = orGroup.Split([" AND "], StringSplitOptions.None)
                .Select(filter =>
                {
                    var parts = filter.Split([' '], 3);
                    if (parts.Length != 3)
                    {
                        throw new ArgumentException(
                            $"Invalid FilterBy string '{filterBy}'. Filter By Format: PropertyName Operator Value");
                    }

                    var propertyName = parts[0];
                    Enum.TryParse<FilterOperator>(parts[1], true, out var filterOperator);
                    var value = ParseValue(parts[2]);

                    return new FilterByInfo
                    {
                        PropertyName = propertyName,
                        Operator = filterOperator,
                        Value = value,
                        IsAndCondition = true
                    };
                }).ToList();

            // Mark the first condition of each OR group
            if (andConditions.Any())
            {
                andConditions[0].IsAndCondition = false;
            }

            foreach (var condition in andConditions)
            {
                yield return condition;
            }
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
    public bool IsAndCondition { get; set; }
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