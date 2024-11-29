using RestSearch.Sorting;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace RestSearch.Json;

internal class SortConverter<TModel> : JsonConverter<SortItem<TModel>>
{
    public override SortItem<TModel>? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
        {
            return null;
        }

        if (reader.TokenType != JsonTokenType.StartObject)
        {
            throw new JsonException();
        }

        string? fieldName = null;
        SortDirection? sortDirection = null;

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
                    case "sort":
                        sortDirection = JsonSerializer.Deserialize<SortDirection?>(ref reader, options);
                        break;
                }
            }
        }

        if (fieldName == null)
        {
            throw new JsonException("'field' must be provided.");
        }

        var fieldDefinition = SearchFieldDefinitionProvider.GetByName<TModel>(fieldName)
            ?? throw new JsonException("'field' does not match any allowed field names.");

        if (fieldDefinition.IsSortingAllowed == false)
        {
            throw new JsonException($"Sorting is not allowed on field '{fieldName}'.");
        }

        return new SortItem<TModel>
        {
            Field = fieldName,
            Sort = sortDirection
        };
    }

    public override void Write(Utf8JsonWriter writer, SortItem<TModel> value, JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }
}
