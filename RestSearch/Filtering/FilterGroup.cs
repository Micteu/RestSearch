namespace RestSearch.Filtering;

/// <summary>
/// Contains a group of filters combined using the logic operator.
/// </summary>
/// <typeparam name="T">Represents the model class containing a field on which the filters are applied.</typeparam>
public class FilterGroup<T> : Filter<T>
{
    public List<Filter<T>> Items { get; set; } = [];

    public LogicOperator LogicOperator { get; set; } = LogicOperator.And;
}
