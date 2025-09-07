namespace RestSearch.Filtering;

/// <summary>
/// Represents a filter against a single field. The operator does not require a value.
/// </summary>
/// <typeparam name="T">Represents the model class containing a field on which the filter is applied.</typeparam>
public class FieldFilter<T> : Filter<T>
{
    /// <summary>
    /// The name of the field upon which the filter is applied.
    /// </summary>
    public string Field { get; set; } = string.Empty;

    /// <summary>
    /// Operator to be applied against the field.
    /// </summary>
    public FilterOperator Operator { get; set; }
}

/// <summary>
/// Represents a filter against a single field. The operator requires a value.
/// </summary>
/// <typeparam name="TModel">Represents the model class containing a field on which the filter is applied.</typeparam>
/// <typeparam name="TValue">The type of the value being compared.</typeparam>
public class FieldFilter<TModel, TValue> : FieldFilter<TModel>
{
    /// <summary>
    /// The value to be compared against the field using the specified operator.
    /// </summary>
    public TValue Value { get; set; }
}
