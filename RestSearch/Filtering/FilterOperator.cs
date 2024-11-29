using System.Text.Json.Serialization;

namespace RestSearch.Filtering;

/// <summary>
/// Operations that can be done for filtering data.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum FilterOperator
{
    /// <summary>
    /// Used with any data type.
    /// </summary>
    Equals = 1,
    /// <summary>
    /// Used with any data type.
    /// </summary>
    DoesNotEqual,
    /// <summary>
    /// Used with strings.
    /// </summary>
    Contains,
    /// <summary>
    /// Used with strings.
    /// </summary>
    DoesNotContain,
    /// <summary>
    /// Used with strings.
    /// </summary>
    StartsWith,
    /// <summary>
    /// Used with strings.
    /// </summary>
    EndsWith,
    /// <summary>
    /// Used with dates, amounts, and other numeric types.
    /// </summary>
    [JsonStringEnumMemberName("=")]
    NumericEquals,
    /// <summary>
    /// Used with dates, amounts, and other numeric types.
    /// </summary>
    [JsonStringEnumMemberName("!=")]
    NumericDoesNotEqual,
    /// <summary>
    /// Used with dates, amounts, and other numeric types.
    /// </summary>
    [JsonStringEnumMemberName(">")]
    GreaterThan,
    /// <summary>
    /// Used with dates, amounts, and other numeric types.
    /// </summary>
    [JsonStringEnumMemberName(">=")]
    GreaterThanOrEqualTo,
    /// <summary>
    /// Used with dates, amounts, and other numeric types.
    /// </summary>
    [JsonStringEnumMemberName("<")]
    LessThan,
    /// <summary>
    /// Used with dates, amounts, and other numeric types.
    /// </summary>
    [JsonStringEnumMemberName("<=")]
    LessThanOrEqualTo,
    /// <summary>
    /// Used with any nullable type. With strings, also checks that it is an empty string.
    /// </summary>
    IsEmpty,
    /// <summary>
    /// Used with any nullable type. With strings, also checks that it is not an empty string.
    /// </summary>
    IsNotEmpty,
    /// <summary>
    /// Used with most data types. Allows for matching multiple values.
    /// </summary>
    IsAnyOf,
    /// <summary>
    /// Used with dates.
    /// </summary>
    Is,
    /// <summary>
    /// Used with dates.
    /// </summary>
    Not,
    /// <summary>
    /// Used with dates.
    /// </summary>
    After,
    /// <summary>
    /// Used with dates.
    /// </summary>
    OnOrAfter,
    /// <summary>
    /// Used with dates.
    /// </summary>
    Before,
    /// <summary>
    /// Used with dates.
    /// </summary>
    OnOrBefore
}
