using RestSearch.Filtering;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace RestSearch;

internal static class SearchFieldDefinitionProvider
{
    private static readonly Dictionary<Type, object> typeFieldDefinitions = [];

    internal static SearchFieldDefinition<T>? GetByName<T>(string fieldName)
    {
        var type = typeof(T);
        if (!typeFieldDefinitions.TryGetValue(type, out var fieldDefinitions))
        {
            throw new Exception($"Field definitions were not initialized for type {type}.");
        }

        if (fieldDefinitions is not Dictionary<string, SearchFieldDefinition<T>> correctTypeFieldDefinitionDictionary)
        {
            throw new Exception($"Found field definitions for type {type}, but it was instead type {fieldDefinitions.GetType()}");
        }

        if (correctTypeFieldDefinitionDictionary.TryGetValue(fieldName, out var fieldDefinition))
        {
            return fieldDefinition;
        }
        return null;
    }

    internal static void AddType<T>(JsonSerializerOptions serializerOptions)
    {
        var type = typeof(T);
        if (!typeFieldDefinitions.ContainsKey(type))
        {
            var fieldDefs = GetFieldDefinitionsForType<T>(type, serializerOptions) as object;
            typeFieldDefinitions[type] = fieldDefs;
        }
    }

    private static Dictionary<string, SearchFieldDefinition<TModel>> GetFieldDefinitionsForType<TModel>(Type type, JsonSerializerOptions serializerOptions)
    {
        var properties = type.GetProperties();
        var fieldDefinitions = new Dictionary<string, SearchFieldDefinition<TModel>>();
        foreach (var property in properties)
        {
            var fieldDefinition = new SearchFieldDefinition<TModel>
            {
                ClrFieldName = property.Name,
                FieldName = GetJsonFieldName(property, serializerOptions),
                IsSortingAllowed = true
            };

            if (property.PropertyType == typeof(int))
            {
                AddNumericOperators<TModel, int>(fieldDefinition, property.Name);
                fieldDefinition.DeserializeFilterItem = (CreateFilter<TModel, int>);
            }
            else if (property.PropertyType == typeof(decimal))
            {
                AddNumericOperators<TModel, decimal>(fieldDefinition, property.Name);
                fieldDefinition.DeserializeFilterItem = (CreateFilter<TModel, decimal>);
            }
            else if (property.PropertyType == typeof(bool))
            {
                fieldDefinition.FilterOperators[FilterOperator.Is] = (filter, param) =>
                        GetAppliedBinaryComparison<TModel, bool>(filter, param, property.Name, Expression.Equal);
                fieldDefinition.DeserializeFilterItem = (CreateFilter<TModel, bool>);
            }
            else if (property.PropertyType == typeof(DateOnly))
            {
                AddDateOperators<TModel, DateOnly>(fieldDefinition, property.Name);
                fieldDefinition.DeserializeFilterItem = (CreateFilter<TModel, DateOnly>);
            }
            else if (property.PropertyType == typeof(DateTime))
            {
                AddDateOperators<TModel, DateTime>(fieldDefinition, property.Name);
                fieldDefinition.DeserializeFilterItem = (CreateFilter<TModel, DateTime>);
            }
            else if (property.PropertyType == typeof(DateTimeOffset))
            {
                AddDateOperators<TModel, DateTimeOffset>(fieldDefinition, property.Name);
                fieldDefinition.DeserializeFilterItem = (CreateFilter<TModel, DateTimeOffset>);
            }
            else if (property.PropertyType == typeof(Guid))
            { // Deviating from MUI's field types because UUIDs are weird. They're like a number passed as a string, but > and < (usually) mean nothing on them.
                fieldDefinition.FilterOperators[FilterOperator.Equals] = (filter, param) =>
                        GetAppliedBinaryComparison<TModel, Guid>(filter, param, property.Name, Expression.Equal);
                fieldDefinition.FilterOperators[FilterOperator.DoesNotEqual] = (filter, param) =>
                        GetAppliedBinaryComparison<TModel, Guid>(filter, param, property.Name, Expression.NotEqual);
                fieldDefinition.DeserializeFilterItem = (CreateFilter<TModel, Guid>);
            }
            else
            { // Assume string?
                fieldDefinition.FilterOperators[FilterOperator.Contains] = (filter, param) =>
                {
                    if (filter is FieldFilter<TModel, string> stringFilter)
                    {
                        var member = Expression.Property(param, property.Name);
                        var constant = Expression.Constant(stringFilter.Value);
                        return Expression.Call(member, "Contains", Type.EmptyTypes, constant);
                    }
                    return Expression.Empty();
                };
                fieldDefinition.FilterOperators[FilterOperator.DoesNotContain] = (filter, param) =>
                {
                    if (filter is FieldFilter<TModel, string> stringFilter)
                    {
                        var member = Expression.Property(param, property.Name);
                        var constant = Expression.Constant(stringFilter.Value);
                        return Expression.Not(Expression.Call(member, "Contains", Type.EmptyTypes, constant));
                    }
                    return Expression.Empty();
                };
                fieldDefinition.FilterOperators[FilterOperator.Equals] = (filter, param) =>
                    GetAppliedBinaryComparison<TModel, string>(filter, param, property.Name, Expression.Equal);
                fieldDefinition.FilterOperators[FilterOperator.DoesNotEqual] = (filter, param) =>
                    GetAppliedBinaryComparison<TModel, string>(filter, param, property.Name, Expression.NotEqual);
                fieldDefinition.FilterOperators[FilterOperator.StartsWith] = (filter, param) =>
                {
                    if (filter is FieldFilter<TModel, string> stringFilter)
                    {
                        var member = Expression.Property(param, property.Name);
                        var constant = Expression.Constant(stringFilter.Value);
                        return Expression.Call(member, "StartsWith", Type.EmptyTypes, constant);
                    }
                    return Expression.Empty();
                };
                fieldDefinition.FilterOperators[FilterOperator.EndsWith] = (filter, param) =>
                {
                    if (filter is FieldFilter<TModel, string> stringFilter)
                    {
                        var member = Expression.Property(param, property.Name);
                        var constant = Expression.Constant(stringFilter.Value);
                        return Expression.Call(member, "EndsWith", Type.EmptyTypes, constant);
                    }
                    return Expression.Empty();
                };
                fieldDefinition.FilterOperators[FilterOperator.IsEmpty] = (filter, param) =>
                {
                    if (filter is FieldFilter<TModel> stringFilter)
                    {
                        var member = Expression.Property(param, property.Name);
                        var emptyExpression = Expression.Equal(member, Expression.Constant(string.Empty));
                        var nullExpression = Expression.Equal(member, Expression.Constant(null));
                        return Expression.OrElse(emptyExpression, nullExpression);
                    }
                    return Expression.Empty();
                };
                fieldDefinition.FilterOperators[FilterOperator.IsNotEmpty] = (filter, param) =>
                {
                    if (filter is FieldFilter<TModel> stringFilter)
                    {
                        var member = Expression.Property(param, property.Name);
                        var emptyExpression = Expression.Equal(member, Expression.Constant(string.Empty));
                        var nullExpression = Expression.Equal(member, Expression.Constant(null));
                        return Expression.Not(Expression.OrElse(emptyExpression, nullExpression));
                    }
                    return Expression.Empty();
                };
                fieldDefinition.DeserializeFilterItem = (CreateFilter<TModel, string>);
            }

            fieldDefinitions[fieldDefinition.FieldName] = fieldDefinition;
        }
        return fieldDefinitions;
    }

    private static FieldFilter<TModel> CreateFilter<TModel, TValue>(ref Utf8JsonReader reader, string fieldName, FilterOperator filterOperator, JsonSerializerOptions serializerOptions)
    {
        if (filterOperator == FilterOperator.IsAnyOf)
        {
            return new FieldFilter<TModel, List<TValue>>
            {
                Field = fieldName,
                Operator = filterOperator,
                Value = JsonSerializer.Deserialize<List<TValue>>(ref reader, serializerOptions)!
            };
        }
        return new FieldFilter<TModel, TValue>
        {
            Field = fieldName,
            Operator = filterOperator,
            Value = JsonSerializer.Deserialize<TValue>(ref reader, serializerOptions)!
        };
    }

    private static string GetJsonFieldName(PropertyInfo propertyInfo, JsonSerializerOptions serializerOptions)
    {
        return propertyInfo.GetCustomAttribute<JsonPropertyNameAttribute>()?.Name
            ?? serializerOptions.PropertyNamingPolicy?.ConvertName(propertyInfo.Name)
            ?? propertyInfo.Name;
    }

    private static void AddNumericOperators<TModel, TField>(SearchFieldDefinition<TModel> fieldDefinition, string propertyName)
    {
        fieldDefinition.FilterOperators[FilterOperator.NumericEquals] = (filter, param) =>
            GetAppliedBinaryComparison<TModel, TField>(filter, param, propertyName, Expression.Equal);
        fieldDefinition.FilterOperators[FilterOperator.NumericDoesNotEqual] = (filter, param) =>
            GetAppliedBinaryComparison<TModel, TField>(filter, param, propertyName, Expression.NotEqual);
        fieldDefinition.FilterOperators[FilterOperator.GreaterThan] = (filter, param) =>
            GetAppliedBinaryComparison<TModel, TField>(filter, param, propertyName, Expression.GreaterThan);
        fieldDefinition.FilterOperators[FilterOperator.GreaterThanOrEqualTo] = (filter, param) =>
            GetAppliedBinaryComparison<TModel, TField>(filter, param, propertyName, Expression.GreaterThanOrEqual);
        fieldDefinition.FilterOperators[FilterOperator.LessThan] = (filter, param) =>
            GetAppliedBinaryComparison<TModel, TField>(filter, param, propertyName, Expression.LessThan);
        fieldDefinition.FilterOperators[FilterOperator.LessThanOrEqualTo] = (filter, param) =>
            GetAppliedBinaryComparison<TModel, TField>(filter, param, propertyName, Expression.LessThanOrEqual);
        fieldDefinition.FilterOperators[FilterOperator.IsEmpty] = (filter, param) =>
            GetNullValueExpression<TModel, TField>(param, propertyName, Expression.Equal);
        fieldDefinition.FilterOperators[FilterOperator.IsNotEmpty] = (filter, param) =>
            GetNullValueExpression<TModel, TField>(param, propertyName, Expression.NotEqual);
    }

    private static void AddDateOperators<TModel, TField>(SearchFieldDefinition<TModel> fieldDefinition, string propertyName)
    {
        fieldDefinition.FilterOperators[FilterOperator.Is] = (filter, param) =>
            GetAppliedBinaryComparison<TModel, TField>(filter, param, propertyName, Expression.Equal);
        fieldDefinition.FilterOperators[FilterOperator.Not] = (filter, param) =>
            GetAppliedBinaryComparison<TModel, TField>(filter, param, propertyName, Expression.NotEqual);
        fieldDefinition.FilterOperators[FilterOperator.After] = (filter, param) =>
            GetAppliedBinaryComparison<TModel, TField>(filter, param, propertyName, Expression.GreaterThan);
        fieldDefinition.FilterOperators[FilterOperator.OnOrAfter] = (filter, param) =>
            GetAppliedBinaryComparison<TModel, TField>(filter, param, propertyName, Expression.GreaterThanOrEqual);
        fieldDefinition.FilterOperators[FilterOperator.Before] = (filter, param) =>
            GetAppliedBinaryComparison<TModel, TField>(filter, param, propertyName, Expression.LessThan);
        fieldDefinition.FilterOperators[FilterOperator.OnOrBefore] = (filter, param) =>
            GetAppliedBinaryComparison<TModel, TField>(filter, param, propertyName, Expression.LessThanOrEqual);
        fieldDefinition.FilterOperators[FilterOperator.IsEmpty] = (filter, param) =>
            GetNullValueExpression<TModel, TField>(param, propertyName, Expression.Equal);
        fieldDefinition.FilterOperators[FilterOperator.IsNotEmpty] = (filter, param) =>
            GetNullValueExpression<TModel, TField>(param, propertyName, Expression.NotEqual);
    }

    /// <summary>
    /// Creates an expression for comparing a property against null.
    /// </summary>
    private static BinaryExpression GetNullValueExpression<TModel, TField>(ParameterExpression param, string propertyName, Func<Expression, Expression, BinaryExpression> comparisonOperation)
    {
        var member = Expression.Property(param, propertyName);
        var constant = Expression.Constant(null);
        return comparisonOperation(member, constant);
    }

    /// <summary>
    /// Creates an expression for comparing stuff against a single value.
    /// </summary>
    /// <typeparam name="TModel">Type of the model containing the property being compared.</typeparam>
    /// <typeparam name="TField">Type of the field being compared.</typeparam>
    /// <param name="filter">The filter whose value is used for the comparison</param>
    /// <param name="modelParam">Expression for the model containing the property being compared.</param>
    /// <param name="propertyName">The CLR name of the property of a model being compared.</param>
    /// <param name="comparisonOperation">What kind of comparison is being made.</param>
    /// <returns>An expression for comparing the value in the filter with values on the model. Or Empty if there was a type mismatch.</returns>
    private static Expression GetAppliedBinaryComparison<TModel, TField>(FieldFilter<TModel> filter, ParameterExpression modelParam, string propertyName, Func<Expression, Expression, BinaryExpression> comparisonOperation)
    {
        var member = Expression.Property(modelParam, propertyName);
        if (filter is FieldFilter<TModel, TField> singleFieldFilter)
        {
            var constant = Expression.Constant(singleFieldFilter.Value);
            return comparisonOperation(member, constant);
        }
        return Expression.Empty();
    }
}
