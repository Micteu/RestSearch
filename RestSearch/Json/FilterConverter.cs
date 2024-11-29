using RestSearch.Filtering;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace RestSearch.Json;

internal class FilterConverter<TModel>() : JsonConverter<Filter<TModel>>
{
    public override Filter<TModel>? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions serializerOptions)
    {
        /*
         * TODO list:
         * Provide a way to block filtering on certain fields.
         * Provide a way to block sorting on certain fields.
         * Add validation. Or in another package?
         * Create results model that also includes total count of items filtered, not using pagination.
         * Set up XML documentation.
         * Set up OpenAPI documentation using attributes.
         * Set up dependency injection?
         * Set up date only?
         * Allow overriding search behavior for different operations on different data types.
         * Add IsAnyOf support.
         * Separate entities from exposed models. Allow incoming searches on the model, but apply the results to the entities.
         * Clean up namespaces, file names, and access modifiers.
         * Add readme.
         * Set up actions and NuGet
         * Check how MUI handles empty. Does an empty string count, not just null?
         */

        if (reader.TokenType == JsonTokenType.Null)
        {
            return null;
        }

        if (reader.TokenType != JsonTokenType.StartObject)
        {
            throw new JsonException();
        }

        string? fieldName = null;
        FilterOperator? filterOperator = null;
        Utf8JsonReader valueReader = default;
        List<Filter<TModel>>? items = null;
        LogicOperator logicOperator = LogicOperator.And;
        while (reader.Read()
               && reader.TokenType != JsonTokenType.EndObject)
        {
            if (reader.TokenType == JsonTokenType.PropertyName)
            {
                var propertyName = reader.GetString()?.ToLowerInvariant();
                reader.Read();
                switch (propertyName)
                {
                    case "field":
                        fieldName = reader.GetString();
                        break;
                    case "operator":
                        filterOperator = JsonSerializer.Deserialize<FilterOperator>(ref reader, serializerOptions);
                        break;
                    case "value":
                        valueReader = reader;
                        break;
                    case "items":
                        items = JsonSerializer.Deserialize<List<Filter<TModel>>>(ref reader, serializerOptions);
                        break;
                    case "logicoperator":
                        logicOperator = JsonSerializer.Deserialize<LogicOperator>(ref reader, serializerOptions);
                        break;
                }
            }
        }

        if (items != null)
        { // Assuming this filter is a group.
            return new FilterGroup<TModel>
            {
                Items = items,
                LogicOperator = logicOperator
            };
        }
        // Not a group. Assuming it is a single filter item.

        if (fieldName == null)
        {
            throw new JsonException("'field' must be provided.");
        }

        var fieldDefinition = SearchFieldDefinitionProvider.GetByName<TModel>(fieldName)
            ?? throw new JsonException("'field' does not match any allowed field names.");

        if (filterOperator == null)
        {
            throw new JsonException("'operator' must be provided.");
        }

        if (!fieldDefinition.FilterOperators.ContainsKey(filterOperator.Value))
        {
            throw new JsonException("'operator' does not match any allowed operators.");
        }

        if (filterOperator is FilterOperator.IsEmpty or FilterOperator.IsNotEmpty)
        {
            return new FieldFilter<TModel> { Field = fieldName, Operator = filterOperator.Value };
        }

        if (valueReader.TokenType == JsonTokenType.None)
        {
            throw new JsonException("'value' is required for the operator.");
        }

        return fieldDefinition.DeserializeFilterItem(ref valueReader, fieldName, filterOperator.Value, serializerOptions);
    }

    public override void Write(Utf8JsonWriter writer, Filter<TModel> value, JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }
}
