using RestSearch.Filtering;

namespace RestSearch.UnitTests.CollectionExtensionsTests.FilterTests;

public class StringFilterTests
{
    [Test]
    public void ApplyFilters_Contains_Works()
    {
        var entities = new List<ExampleEntity>
        {
            new() { MyString = "ppa", MyInteger = 2 },
            new() { MyString = "pp" },
            new() { MyString = "app" },
            new() { MyString = "appb" },
            new() { MyInteger = 5 },
            new() { MyString = "a", MyInteger = 5 }
        }.AsQueryable();
        var filter = new FieldFilter<ExampleEntity, string>
        {
            Field = "myString",
            Operator = FilterOperator.Contains,
            Value = "pp"
        };
        var results = entities.ApplyFilters(filter);
        Assert.Multiple(() =>
        {
            Assert.That(results.Count(), Is.EqualTo(4));
            Assert.That(results.Any(e => e.MyString == "pp"), Is.True);
            Assert.That(results.Any(e => e.MyString == "ppa"), Is.True);
            Assert.That(results.Any(e => e.MyString == "app"), Is.True);
            Assert.That(results.Any(e => e.MyString == "appb"), Is.True);
        });
    }

    [Test]
    public void ApplyFilters_DoesNotContain_Works()
    {
        var entities = new List<ExampleEntity>
        {
            new() { MyString = "ppa", MyInteger = 2 },
            new() { MyString = "pp" },
            new() { MyString = "app" },
            new() { MyString = "appb" },
            new() { MyInteger = 5 },
            new() { MyString = "a", MyInteger = 5 }
        }.AsQueryable();
        var filter = new FieldFilter<ExampleEntity, string>
        {
            Field = "myString",
            Operator = FilterOperator.DoesNotContain,
            Value = "pp"
        };
        var results = entities.ApplyFilters(filter);
        Assert.Multiple(() =>
        {
            Assert.That(results.Count(), Is.EqualTo(2));
            Assert.That(results.Any(e => e.MyString == "a"), Is.True);
            Assert.That(results.Any(e => e.MyString == ""), Is.True);
        });
    }

    [Test]
    public void ApplyFilters_Equals_Works()
    {
        var entities = new List<ExampleEntity>
        {
            new() { MyString = "ppa", MyInteger = 2 },
            new() { MyString = "pp" },
            new() { MyString = "app" },
            new() { MyString = "appb" },
            new() { MyInteger = 5 },
            new() { MyString = "a", MyInteger = 5 }
        }.AsQueryable();
        var filter = new FieldFilter<ExampleEntity, string>
        {
            Field = "myString",
            Operator = FilterOperator.Equals,
            Value = "pp"
        };
        var results = entities.ApplyFilters(filter);
        Assert.Multiple(() =>
        {
            Assert.That(results.Count(), Is.EqualTo(1));
            Assert.That(results.Any(e => e.MyString == "pp"), Is.True);
        });
    }

    [Test]
    public void ApplyFilters_DoesNotEqual_Works()
    {
        var entities = new List<ExampleEntity>
        {
            new() { MyString = "ppa", MyInteger = 2 },
            new() { MyString = "pp" },
            new() { MyString = "app" },
            new() { MyString = "appb" },
            new() { MyInteger = 5 },
            new() { MyString = "a", MyInteger = 5 }
        }.AsQueryable();
        var filter = new FieldFilter<ExampleEntity, string>
        {
            Field = "myString",
            Operator = FilterOperator.DoesNotEqual,
            Value = "pp"
        };
        var results = entities.ApplyFilters(filter);
        Assert.Multiple(() =>
        {
            Assert.That(results.Count(), Is.EqualTo(5));
            Assert.That(results.Any(e => e.MyString == "pp"), Is.False);
        });
    }

    [Test]
    public void ApplyFilters_StartsWith_Works()
    {
        var entities = new List<ExampleEntity>
        {
            new() { MyString = "ppa", MyInteger = 2 },
            new() { MyString = "pp" },
            new() { MyString = "app" },
            new() { MyString = "appb" },
            new() { MyInteger = 5 },
            new() { MyString = "a", MyInteger = 5 }
        }.AsQueryable();
        var filter = new FieldFilter<ExampleEntity, string>
        {
            Field = "myString",
            Operator = FilterOperator.StartsWith,
            Value = "pp"
        };
        var results = entities.ApplyFilters(filter);
        Assert.Multiple(() =>
        {
            Assert.That(results.Count(), Is.EqualTo(2));
            Assert.That(results.Any(e => e.MyString == "pp"), Is.True);
            Assert.That(results.Any(e => e.MyString == "ppa"), Is.True);
        });
    }

    [Test]
    public void ApplyFilters_EndsWith_Works()
    {
        var entities = new List<ExampleEntity>
        {
            new() { MyString = "ppa", MyInteger = 2 },
            new() { MyString = "pp" },
            new() { MyString = "app" },
            new() { MyString = "appb" },
            new() { MyInteger = 5 },
            new() { MyString = "p", MyInteger = 5 }
        }.AsQueryable();
        var filter = new FieldFilter<ExampleEntity, string>
        {
            Field = "myString",
            Operator = FilterOperator.EndsWith,
            Value = "pp"
        };
        var results = entities.ApplyFilters(filter);
        Assert.Multiple(() =>
        {
            Assert.That(results.Count(), Is.EqualTo(2));
            Assert.That(results.Any(e => e.MyString == "pp"), Is.True);
            Assert.That(results.Any(e => e.MyString == "app"), Is.True);
        });
    }

    [Test]
    public void ApplyFilters_IsEmpty_Works()
    {
        var entities = new List<ExampleEntity>
        {
            new() { MyString = "ppa", MyInteger = 2 },
            new() { MyString = "pp" },
            new() { MyString = "app" },
            new() { MyString = "appb" },
            new() { MyInteger = 5 },
            new() { MyString = "a", MyInteger = 5 },
            new() { MyString = null, MyInteger = 5 }
        }.AsQueryable();
        var filter = new FieldFilter<ExampleEntity>
        {
            Field = "myString",
            Operator = FilterOperator.IsEmpty
        };
        var results = entities.ApplyFilters(filter);
        Assert.Multiple(() =>
        {
            Assert.That(results.Count(), Is.EqualTo(2));
            Assert.That(results.Any(e => e.MyString == ""), Is.True);
            Assert.That(results.Any(e => e.MyString == null), Is.True);
        });
    }

    [Test]
    public void ApplyFilters_IsNotEmpty_Works()
    {
        var entities = new List<ExampleEntity>
        {
            new() { MyString = "ppa", MyInteger = 2 },
            new() { MyString = "pp" },
            new() { MyString = "app" },
            new() { MyString = "appb" },
            new() { MyInteger = 5 },
            new() { MyString = "a", MyInteger = 5 },
            new() { MyString = null, MyInteger = 5 }
        }.AsQueryable();
        var filter = new FieldFilter<ExampleEntity>
        {
            Field = "myString",
            Operator = FilterOperator.IsNotEmpty
        };
        var results = entities.ApplyFilters(filter);
        Assert.Multiple(() =>
        {
            Assert.That(results.Count(), Is.EqualTo(5));
            Assert.That(results.Any(e => e.MyString == ""), Is.False);
            Assert.That(results.Any(e => e.MyString == null), Is.False);
        });
    }
}
