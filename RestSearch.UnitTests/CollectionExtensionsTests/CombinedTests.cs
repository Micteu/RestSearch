using RestSearch.Filtering;
using RestSearch.Sorting;

namespace RestSearch.UnitTests.CollectionExtensionsTests;

public class CombinedTests
{
    [Test]
    public void ApplyFilters_ApplySorting_DoesNotCrapTheBed()
    {
        var entities = new List<ExampleEntity>
        {
            new() { MyString = "a", MyInteger = 2 },
            new() { MyString = "b", MyInteger = 1 },
            new() { MyString = "a", MyInteger = 1 },
            new() { MyString = "b", MyInteger = 2 }
        }.AsQueryable();

        var filter = new FieldFilter<ExampleEntity, string>
        {
            Field = "myString",
            Operator = FilterOperator.Equals,
            Value = "a"
        };

        var sortItems = new List<SortItem<ExampleEntity>>
        {
            new() { Field = "myInteger", Sort = SortDirection.Ascending }
        };

        var results = entities.ApplyFilters(filter).ApplySorting(sortItems).ToList();

        Assert.Multiple(() =>
        {
            Assert.That(results, Has.Count.EqualTo(2));
            Assert.That(results[0].MyString, Is.EqualTo("a"));
            Assert.That(results[0].MyInteger, Is.EqualTo(1));
            Assert.That(results[1].MyString, Is.EqualTo("a"));
            Assert.That(results[1].MyInteger, Is.EqualTo(2));
        });
    }
}
