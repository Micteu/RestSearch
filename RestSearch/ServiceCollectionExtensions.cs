using RestSearch.Json;
using System.Text.Json;

namespace RestSearch;

public static class ServiceCollectionExtensions
{
    public static void AddRestSearchConverters<T>(this JsonSerializerOptions serializerOptions)
    {
        serializerOptions.Converters.Add(new FilterConverter<T>());
        serializerOptions.Converters.Add(new SortConverter<T>());
        SearchFieldDefinitionProvider.AddType<T>(serializerOptions);
    }
    public static void AddRestSearchConverters<T1, T2>(this JsonSerializerOptions serializerOptions)
    {
        serializerOptions.Converters.Add(new FilterConverter<T1>());
        serializerOptions.Converters.Add(new SortConverter<T1>());
        SearchFieldDefinitionProvider.AddType<T1>(serializerOptions);
        serializerOptions.Converters.Add(new FilterConverter<T2>());
        serializerOptions.Converters.Add(new SortConverter<T2>());
        SearchFieldDefinitionProvider.AddType<T2>(serializerOptions);
    }
    public static void AddRestSearchConverters<T1, T2, T3>(this JsonSerializerOptions serializerOptions)
    {
        serializerOptions.Converters.Add(new FilterConverter<T1>());
        serializerOptions.Converters.Add(new SortConverter<T1>());
        SearchFieldDefinitionProvider.AddType<T1>(serializerOptions);
        serializerOptions.Converters.Add(new FilterConverter<T2>());
        serializerOptions.Converters.Add(new SortConverter<T2>());
        SearchFieldDefinitionProvider.AddType<T2>(serializerOptions);
        serializerOptions.Converters.Add(new FilterConverter<T3>());
        serializerOptions.Converters.Add(new SortConverter<T3>());
        SearchFieldDefinitionProvider.AddType<T3>(serializerOptions);
    }
}
