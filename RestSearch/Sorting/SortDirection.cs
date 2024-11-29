using System.Text.Json.Serialization;

namespace RestSearch.Sorting;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum SortDirection
{
    /// <summary>
    /// Sort 0, 1, 2 or a, b, c.
    /// </summary>
    [JsonStringEnumMemberName("asc")]
    Ascending = 1,
    /// <summary>
    /// Sort 2, 1, 0 or c, b, a
    /// </summary>
    [JsonStringEnumMemberName("desc")]
    Descending
}
