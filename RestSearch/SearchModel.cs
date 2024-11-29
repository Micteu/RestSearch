using RestSearch.Filtering;
using RestSearch.Sorting;

namespace RestSearch;

/// <summary>
/// The contents when a search request is made.
/// </summary>
public class SearchModel<T>
{
    /// <summary>
    /// Array of groups of filters. If not provided, no filtering will be done.
    /// </summary>
    public Filter<T>? Filtering { get; set; }

    /// <summary>
    /// Array of sorting to be applied to the data, in order. The first item in the list will be applied first. Then
    /// the next item will be applied to sort any data that had equal values in the first item. Then the next, etc.
    /// </summary>
    public List<SortItem<T>> Sorting { get; set; } = [];

    /// <summary>
    /// Pagination to apply after filtering and sorting. If not provided, no pagination will be used.
    /// </summary>
    public Pagination? Pagination { get; set; }
}
