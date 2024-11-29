using RestSearch.Sorting;
using System.Text.Json;

namespace RestSearch.UnitTests.Json;

public class SortConverterTests
{
    [TestCase("{ \"sortDirection\": \"asc\" }", TestName = "Bad Input. Field Missing")]
    [TestCase("{ \"field\": null, \"sort\": \"asc\" }", TestName = "Bad Input. Field Null")]
    [TestCase("{ \"field\": \"myInteger\", \"sort\": \"bloop\" }", TestName = "Bad Input. SortDirection not in enum")]
    public void BadInput_ThrowsException(string inputJson)
    {
        Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<SortItem<ExampleEntity>>(inputJson, Setup.SerializerOptions));
    }

    [TestCase("{ \"field\": \"myString\", \"sort\": \"asc\" }", "myString", SortDirection.Ascending, TestName = "Good Input. Ascending")]
    [TestCase("{ \"field\": \"myInteger\", \"sort\": \"desc\" }", "myInteger", SortDirection.Descending, TestName = "Good Input. Descending")]
    [TestCase("{ \"field\": \"myString\", \"sort\": null }", "myString", null, TestName = "Good Input. Null")]
    public void GoodInput_DeserializesProperly(string inputJson, string expectedField, SortDirection? expectedSort)
    {
        var result = JsonSerializer.Deserialize<SortItem<ExampleEntity>>(inputJson, Setup.SerializerOptions);
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result!.Field, Is.EqualTo(expectedField));
            Assert.That(result.Sort, Is.EqualTo(expectedSort));
        });
    }
}
