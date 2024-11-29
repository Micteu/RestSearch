namespace RestSearch.Filtering;

/// <summary>
/// Represents a filter against a single field. The operator does not require a value.
/// </summary>
/// <typeparam name="T">Represents the model class containing a field on which the filter is applied.</typeparam>
public class FieldFilter<T> : Filter<T>
{
    public string Field { get; set; } = string.Empty;
    public FilterOperator Operator { get; set; }
}

/// <summary>
/// Represents a filter against a single field. The operator requires a value.
/// </summary>
/// <typeparam name="TModel">Represents the model class containing a field on which the filter is applied.</typeparam>
/// <typeparam name="TValue">The type of the value being compared.</typeparam>
public class FieldFilter<TModel, TValue> : FieldFilter<TModel>
{
    public TValue Value { get; set; }
}
