namespace RestSearch.Sorting;

/// <summary>
/// Represents sorting against a single field.
/// </summary>
public class SortItem<TModel>
{
    /// <summary>
    /// The name of the field to be sorted by.
    /// </summary>
    public string Field { get; set; } = string.Empty;

    /// <summary>
    /// Changes whether to sort ascending or descending. If not provided, this sort item will not be applied.
    /// </summary>
    public SortDirection? Sort { get; set; }
}
