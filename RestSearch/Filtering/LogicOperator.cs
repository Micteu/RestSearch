using System.Text.Json.Serialization;

namespace RestSearch.Filtering;

/// <summary>
/// Used for combining multiple filters.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum LogicOperator
{
    /// <summary>
    /// Data must match all filters.
    /// </summary>
    And = 1,
    /// <summary>
    /// Data must match at least one filter.
    /// </summary>
    Or = 2
}
