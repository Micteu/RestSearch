using RestSearch.Json;
using System.Text.Json;

namespace RestSearch;

/// <summary>
/// Contains extension methods for adding RestSearch converters to JsonSerializerOptions.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Sets up RestSearch for the specified type.
    /// </summary>
    /// <typeparam name="TModel">The model or entity that should allow searches.</typeparam>
    /// <param name="serializerOptions">used for determining how to deserialize incoming JSON requests</param>
    public static void AddRestSearchConverters<TModel>(this JsonSerializerOptions serializerOptions)
    {
        serializerOptions.Converters.Add(new FilterConverter<TModel>());
        serializerOptions.Converters.Add(new SortConverter<TModel>());
        SearchFieldDefinitionProvider.AddType<TModel>(serializerOptions);
    }

    /// <summary>
    /// For convenience when you have two models in the same API that need to support RestSearch.
    /// </summary>
    public static void AddRestSearchConverters<T1, T2>(this JsonSerializerOptions serializerOptions)
    {
        serializerOptions.AddRestSearchConverters<T1>();
        serializerOptions.AddRestSearchConverters<T2>();
    }

    /// <summary>
    /// For convenience when you have three models in the same API that need to support RestSearch.
    /// </summary>
    public static void AddRestSearchConverters<T1, T2, T3>(this JsonSerializerOptions serializerOptions)
    {
        serializerOptions.AddRestSearchConverters<T1>();
        serializerOptions.AddRestSearchConverters<T2>();
        serializerOptions.AddRestSearchConverters<T3>();
    }
}
