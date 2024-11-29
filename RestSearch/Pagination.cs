namespace RestSearch;

/// <summary>
/// How many records should be returned.
/// </summary>
public class Pagination
{
    /// <summary>
    /// Indicates which page to retrieve. 0 is the first page.
    /// </summary>
    public int Page { get; set; } = 0;

    /// <summary>
    /// Number of records in a page. 0 or negative means no limit.
    /// </summary>
    public int PageSize { get; set; } = 100;
}
