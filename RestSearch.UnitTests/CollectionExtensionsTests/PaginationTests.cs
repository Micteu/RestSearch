namespace RestSearch.UnitTests.CollectionExtensionsTests;

public class PaginationTests
{
    [Test]
    public void ApplyPagination_NullPagination_ReturnsAll()
    {
        var entities = new List<ExampleEntity>
        {
            new() { MyString = "a", MyInteger = 1 },
            new() { MyString = "b", MyInteger = 2 }
        }.AsQueryable();

        var results = entities.ApplyPagination(null).ToList();

        Assert.That(results, Has.Count.EqualTo(2));
    }

    [TestCase(0)]
    [TestCase(-1)]
    [TestCase(int.MinValue)]
    public void ApplyPagination_NonpositivePageSize_ReturnsAll(int pageSize)
    {
        var entities = new List<ExampleEntity>
        {
            new() { MyString = "a", MyInteger = 1 },
            new() { MyString = "b", MyInteger = 2 }
        }.AsQueryable();

        var pagination = new Pagination { Page = 0, PageSize = pageSize };

        var results = entities.ApplyPagination(pagination).ToList();

        Assert.That(results, Has.Count.EqualTo(2));
    }
    
    [Test]
    public void ApplyPagination_SmallPage_ReturnsCorrectPage()
    {
        var entities = new List<ExampleEntity>
        {
            new() { MyString = "a", MyInteger = 1 },
            new() { MyString = "b", MyInteger = 2 },
            new() { MyString = "c", MyInteger = 3 },
            new() { MyString = "d", MyInteger = 4 },
            new() { MyString = "e", MyInteger = 5 }
        }.AsQueryable();

        var pagination = new Pagination { Page = 1, PageSize = 2 };

        var results = entities.ApplyPagination(pagination).ToList();

        Assert.Multiple(() =>
        {
            Assert.That(results, Has.Count.EqualTo(2));
            Assert.That(results[0].MyString, Is.EqualTo("c"));
            Assert.That(results[1].MyString, Is.EqualTo("d"));
        });
    }

    [Test]
    public void ApplyPagination_LargePage_ReturnsEmpty()
    {
        var entities = new List<ExampleEntity>
        {
            new() { MyString = "a", MyInteger = 1 },
            new() { MyString = "b", MyInteger = 2 },
            new() { MyString = "c", MyInteger = 3 },
            new() { MyString = "d", MyInteger = 4 },
            new() { MyString = "e", MyInteger = 5 }
        }.AsQueryable();

        var pagination = new Pagination { Page = 10, PageSize = 2 };

        var results = entities.ApplyPagination(pagination).ToList();

        Assert.That(results, Has.Count.EqualTo(0));
    }
}
