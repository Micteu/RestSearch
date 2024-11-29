using RestSearch.Filtering;
using System.Linq.Expressions;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace RestSearch;

public delegate Expression<Func<TModel, bool>> ApplyFilter<TModel>(FieldFilter<TModel> filter);
public delegate FieldFilter<TModel> Deserializer<TModel>(ref Utf8JsonReader reader, string fieldName, FilterOperator filterOperator, JsonSerializerOptions serializerOptions);

public class SearchFieldDefinition<TModel>
{
    public string FieldName { get; set; } = "";

    [JsonIgnore]
    public string ClrFieldName { get; set; } = "";

    public List<FilterOperator> FilterOperatorsAllowed { get; set; } = [];

    [JsonIgnore]
    public Dictionary<FilterOperator, Func<FieldFilter<TModel>, ParameterExpression, Expression>> FilterOperators { get; set; } = [];

    public bool IsSortingAllowed { get; set; } = true;

    [JsonIgnore]
    public Deserializer<TModel> DeserializeFilterItem { get; set; }
}
