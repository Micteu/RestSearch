using System.Text.Json.Serialization;

namespace RestSearch.Filtering;

/// <summary>
/// Used for combining multiple filters.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum LogicOperator
{
    And = 1,
    Or = 2
}
