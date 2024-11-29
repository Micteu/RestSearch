using RestSearch.Filtering;

namespace RestSearch.UnitTests.CollectionExtensionsTests.FilterTests;

public class FilterTests
{
    [TestCase(LogicOperator.Or, 3)]
    [TestCase(LogicOperator.And, 1)]
    public void ApplyFilters_FilterGroup_AppliesProperly(LogicOperator op, int expectedCount)
    {
        var entities = new List<ExampleEntity>
        {
            new() { MyString = "stuff" }, // Does not match.
            new() { MyString = "app" }, // Does not match.
            new() { MyString = "a" }, // Matches on Or.
            new() { MyInteger = 5 }, // Matches on Or.
            new() { MyString = "a", MyInteger = 5 } // Matches on And or Or.
        }.AsQueryable();

        var filters = new FilterGroup<ExampleEntity>
        {
            Items =
            [
                new FieldFilter<ExampleEntity, int>
                {
                    Field = "myInteger",
                    Operator = FilterOperator.NumericEquals,
                    Value = 5
                },
                new FieldFilter<ExampleEntity, string>
                {
                    Field = "myString",
                    Operator = FilterOperator.Equals,
                    Value = "a"
                }
            ],
            LogicOperator = op
        };

        var results = entities.ApplyFilters(filters);

        Assert.That(results.Count(), Is.EqualTo(expectedCount));
    }

    [Test]
    public void ApplyFilters_NestedFilterGroup_AppliesProperly()
    {
        var entities = new List<ExampleEntity>
        {
            new() { MyString = "a", MyInteger = 2 }, // Does not match.
            new() { MyString = "app" }, // Does not match.
            new() { MyString = "a" }, // Matches.
            new() { MyInteger = 5 }, // Does not match.
            new() { MyString = "a", MyInteger = 5 } // Matches.
        }.AsQueryable();

        var filters = new FilterGroup<ExampleEntity>
        {
            LogicOperator = LogicOperator.And,
            Items =
            [
                new FieldFilter<ExampleEntity, string>
                {
                    Field = "myString",
                    Operator = FilterOperator.Equals,
                    Value = "a"
                },
                new FilterGroup<ExampleEntity>
                {
                    LogicOperator = LogicOperator.Or,
                    Items =
                    [
                        new FieldFilter<ExampleEntity, int>
                        {
                            Field = "myInteger",
                            Operator = FilterOperator.NumericEquals,
                            Value = 5
                        },
                        new FieldFilter<ExampleEntity, int>
                        {
                            Field = "myInteger",
                            Operator = FilterOperator.NumericEquals,
                            Value = 0
                        }
                    ]
                }
            ]
        };

        var results = entities.ApplyFilters(filters);

        Assert.That(results.Count(), Is.EqualTo(2));
    }

    [Test]
    public void ApplyFilters_NullFilter_NoChanges()
    {
        var entities = new List<ExampleEntity>
        {
            new() { MyString = "a", MyInteger = 2 },
            new() { MyString = "app" },
            new() { MyString = "a" },
            new() { MyInteger = 5 },
            new() { MyString = "a", MyInteger = 5 }
        }.AsQueryable();

        var results = entities.ApplyFilters(null);

        Assert.That(results.Count(), Is.EqualTo(entities.Count()));
    }
}
