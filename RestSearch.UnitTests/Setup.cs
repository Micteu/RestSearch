using RestSearch.Json;
using System.Text.Json;

namespace RestSearch.UnitTests;

/// <summary>
/// Runs once before any tests are run to set up the SearchFieldDefinitionProvider.
/// </summary>
[SetUpFixture]
public class Setup
{
    [OneTimeSetUp]
    public void RunBeforeAnyTests()
    {
        SearchFieldDefinitionProvider.AddType<ExampleEntity>(SerializerOptions);
    }

    public static JsonSerializerOptions SerializerOptions => new JsonSerializerOptions(JsonSerializerDefaults.Web)
    {
        Converters =
        {
            new FilterConverter<ExampleEntity>(),
            new SortConverter<ExampleEntity>()
        }
    };
}
