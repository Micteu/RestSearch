using RestSearch.Filtering;
using System.Linq.Expressions;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace RestSearch;

/// <summary>
/// Represents a method that deserializes a filter from JSON into a <see cref="FieldFilter{TModel}"/>.
/// </summary>
public delegate FieldFilter<TModel> Deserializer<TModel>(ref Utf8JsonReader reader, string fieldName, FilterOperator filterOperator, JsonSerializerOptions serializerOptions);

/// <summary>
/// Defines the properties of a searchable field in a model
/// </summary>
/// <typeparam name="TModel">The type of the model in which the field appears, not the type of the field.</typeparam>
public class SearchFieldDefinition<TModel>
{
    /// <summary>
    /// The name of the field as it would appear in JSON.
    /// </summary>
    public string FieldName { get; set; } = "";

    /// <summary>
    /// The name of the field as it appears in the CLR or .NET object.
    /// </summary>
    [JsonIgnore]
    public string ClrFieldName { get; set; } = "";

    /// <summary>
    /// Dictionary of filter operators. Each entry is a method that takes in a filter and a parameter expression, returning an expression describing the operation for that operator.
    /// </summary>
    [JsonIgnore]
    public Dictionary<FilterOperator, Func<FieldFilter<TModel>, ParameterExpression, Expression>> FilterOperators { get; set; } = [];

    /// <summary>
    /// Defaults to true. If false, this field cannot be used in sorting.
    /// </summary>
    public bool IsSortingAllowed { get; set; } = true;

    /// <summary>
    /// A method to deserialize a filter item for this field from JSON.
    /// </summary>
    [JsonIgnore]
    public Deserializer<TModel> DeserializeFilterItem { get; set; }
}
