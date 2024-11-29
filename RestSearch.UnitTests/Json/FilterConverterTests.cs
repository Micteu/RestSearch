using RestSearch.Filtering;
using System.Text.Json;

namespace RestSearch.UnitTests.Json;

public class FilterConverterTests
{
    [TestCase("{ \"operator\": \"isEmpty\" }", TestName = "Single Filter. Bad Input. Field Missing")]
    [TestCase("{ \"field\": null, \"operator\": \"isEmpty\" }", TestName = "Single Filter. Bad Input. Field Null")]
    [TestCase("{ \"field\": undefined, \"operator\": \"isEmpty\" }", TestName = "Single Filter. Bad Input. Field Undefined")]
    [TestCase("{ \"field\": \"\", \"operator\": \"isEmpty\" }", TestName = "Single Filter. Bad Input. Field Empty")]
    [TestCase("{ \"field\": \"warglblargl\", \"operator\": \"isEmpty\" }", TestName = "Single Filter. Bad Input. Field Not Matching")]
    [TestCase("{ \"field\": \"myString\" }", TestName = "Single Filter. Bad Input. Operator Missing")]
    [TestCase("{ \"field\": \"myString\", \"operator\": null }", TestName = "Single Filter. Bad Input. Operator Null")]
    [TestCase("{ \"field\": \"myString\", \"operator\": undefined }", TestName = "Single Filter. Bad Input. Operator Undefined")]
    [TestCase("{ \"field\": \"myString\", \"operator\": \"\" }", TestName = "Single Filter. Bad Input. Operator Empty")]
    [TestCase("{ \"field\": \"myString\", \"operator\": \"warglblargl\" }", TestName = "Single Filter. Bad Input. Operator Not Matching")]
    [TestCase("{ \"field\": \"myString\", \"operator\": \"is\" }", TestName = "Single Filter. Bad Input. Missing Value")]
    public void SingleFilter_BadInput_ThrowsException(string inputJson)
    {
        Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<Filter<ExampleEntity>>(inputJson, Setup.SerializerOptions));
    }

    [TestCase("{ \"items\": \"stuff\" }", TestName = "FilterGroup. Bad Input. Items string type.")]
    [TestCase("{ \"items\": true }", TestName = "FilterGroup. Bad Input. Items boolean type.")]
    public void FilterGroup_BadInput_ThrowsException(string inputJson)
    {
        Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<Filter<ExampleEntity>>(inputJson, Setup.SerializerOptions));
    }

    [TestCase("{ \"field\": \"myString\", \"operator\": \"isEmpty\" }", TestName = "Single Filter. Good Input. isEmpty")]
    [TestCase("{ \"field\": \"myString\", \"operator\": \"equals\", \"value\": \"stuff\" }", TestName = "Single Filter. Good Input. equals")]
    public void FilterGroup_GoodInput_NoException(string inputJson)
    {
        var result = JsonSerializer.Deserialize<Filter<ExampleEntity>>(inputJson, Setup.SerializerOptions);

        Assert.That(result, Is.Not.Null);
    }
}
