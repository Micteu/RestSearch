namespace RestSearch.Filtering;

/// <summary>
/// Contains a group of filters combined using the logic operator.
/// </summary>
/// <typeparam name="T">Represents the model class containing a field on which the filters are applied.</typeparam>
public class FilterGroup<T> : Filter<T>
{
    /// <summary>
    /// The filters to be combined using the logic operator. Each item could be another group or a filter against a single field.
    /// </summary>
    public List<Filter<T>> Items { get; set; } = [];

    /// <summary>
    /// How to combine the filters in this group. Defaults to AND.
    /// </summary>
    public LogicOperator LogicOperator { get; set; } = LogicOperator.And;
}
