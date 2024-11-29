using RestSearch.Sorting;

namespace RestSearch.UnitTests.CollectionExtensionsTests;

public class SortingTests
{
    [TestCase("myString", SortDirection.Ascending, "a")]
    [TestCase("myString", SortDirection.Descending, "b")]
    [TestCase("myInteger", SortDirection.Ascending, "a")]
    [TestCase("myInteger", SortDirection.Descending, "b")]
    public void ApplySorting_AppliesProperly(string fieldName, SortDirection sort, string expectedFirstMyStringValue)
    {
        var entities = new List<ExampleEntity>
        {
            new() { MyString = "a", MyInteger = 1 },
            new() { MyString = "b", MyInteger = 2 }
        }.AsQueryable();

        var sortItem = new SortItem<ExampleEntity> { Field = fieldName, Sort = sort };

        var results = entities.ApplySorting([sortItem]).ToList();

        Assert.That(results[0].MyString, Is.EqualTo(expectedFirstMyStringValue));
    }

    [Test]
    public void ApplySorting_IgnoresBadFilterItems()
    {
        var entities = new List<ExampleEntity>
        {
            new() { MyString = "b", MyInteger = 2 },
            new() { MyString = "a", MyInteger = 1 }
        }.AsQueryable();

        var sortItems = new List<SortItem<ExampleEntity>>
        {
            new() { Field = "myStrong", Sort = SortDirection.Ascending }, // Misspelled field, should be ignored.
            new() { Field = "myInteger", Sort = null } // No sort, should be ignored.
        };

        var results = entities.ApplySorting(sortItems).ToList();

        Assert.Multiple(() =>
        {
            Assert.That(results[0].MyString, Is.EqualTo("b"));
            Assert.That(results[1].MyString, Is.EqualTo("a"));
        });
    }

    [Test]
    public void ApplySorting_MultipleSort_SecondSortAscending_AppliesProperly()
    {
        var entities = new List<ExampleEntity>
        {
            new() { MyString = "a", MyInteger = 2 },
            new() { MyString = "b", MyInteger = 2 },
            new() { MyString = "a", MyInteger = 1 },
            new() { MyString = "b", MyInteger = 1 }
        }.AsQueryable();

        var sortItems = new List<SortItem<ExampleEntity>>
        {
            new() { Field = "myString", Sort = SortDirection.Ascending },
            new() { Field = "myInteger", Sort = SortDirection.Ascending }
        };

        var results = entities.ApplySorting(sortItems).ToList();

        Assert.Multiple(() =>
        {
            Assert.That(results[0].MyString, Is.EqualTo("a"));
            Assert.That(results[0].MyInteger, Is.EqualTo(1));
            Assert.That(results[1].MyString, Is.EqualTo("a"));
            Assert.That(results[1].MyInteger, Is.EqualTo(2));
            Assert.That(results[2].MyString, Is.EqualTo("b"));
            Assert.That(results[2].MyInteger, Is.EqualTo(1));
            Assert.That(results[3].MyString, Is.EqualTo("b"));
            Assert.That(results[3].MyInteger, Is.EqualTo(2));
        });
    }

    [Test]
    public void ApplySorting_MultipleSort_SecondSortDescending_AppliesProperly()
    {
        var entities = new List<ExampleEntity>
        {
            new() { MyString = "a", MyInteger = 2 },
            new() { MyString = "b", MyInteger = 1 },
            new() { MyString = "a", MyInteger = 1 },
            new() { MyString = "b", MyInteger = 2 }
        }.AsQueryable();

        var sortItems = new List<SortItem<ExampleEntity>>
        {
            new() { Field = "myString", Sort = SortDirection.Ascending },
            new() { Field = "myInteger", Sort = SortDirection.Descending }
        };

        var results = entities.ApplySorting(sortItems).ToList();

        Assert.Multiple(() =>
        {
            Assert.That(results[0].MyString, Is.EqualTo("a"));
            Assert.That(results[0].MyInteger, Is.EqualTo(2));
            Assert.That(results[1].MyString, Is.EqualTo("a"));
            Assert.That(results[1].MyInteger, Is.EqualTo(1));
            Assert.That(results[2].MyString, Is.EqualTo("b"));
            Assert.That(results[2].MyInteger, Is.EqualTo(2));
            Assert.That(results[3].MyString, Is.EqualTo("b"));
            Assert.That(results[3].MyInteger, Is.EqualTo(1));
        });
    }
}
