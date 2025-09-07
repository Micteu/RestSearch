RestSearch

Add filtering, sorting, and pagination in ASP.NET Core Web API with minimal overhead. The request body structure is based on objects in the MUI DataGrid, but this library is not limited to backends receiving information from apps using MUI.

## Why not use OData?
OData is definitely more mature--I know parents younger than it. But one of the issues is that the query must be formatted as a string, not a JSON object. There are not many front-end libraries that can generate OData queries.

## Installation

```bash
dotnet add package RestSearch
```

## Setup and Usage

After installing the package, set up injection in Program.cs:
```csharp
builder.Services.AddControllers()
    .AddJsonOptions(c => c.JsonSerializerOptions.AddRestSearchConverters<WeatherForecast>());
```
Okay, that's a little gross. I'll work on something cleaner later.

Next add a search endpoint that takes in a typed `SearchModel` body, where the type is your entity. Then use `ApplyFilters`, `ApplySorting`, and `ApplyPagination` against an IQueryable.
```csharp
[HttpPost(Name = "PostTesterEndpoint")]
public IEnumerable<WeatherForecast> Search([FromBody]SearchModel<WeatherForecast> searchBody)
{
    var forecasts = new List<WeatherForecast>
    {
        new WeatherForecast{ Date = DateOnly.FromDateTime(DateTime.Now.AddDays(1)), TemperatureC = 25, Summary = "Warm" },
        new WeatherForecast{ Date = DateOnly.FromDateTime(DateTime.Now.AddDays(2)), TemperatureC = 30, Summary = "Hot" },
        new WeatherForecast{ Date = DateOnly.FromDateTime(DateTime.Now.AddDays(3)), TemperatureC = 15, Summary = "Cool" },
        new WeatherForecast{ Date = DateOnly.FromDateTime(DateTime.Now.AddDays(4)), TemperatureC = 5, Summary = "Chilly" },
        new WeatherForecast{ Date = DateOnly.FromDateTime(DateTime.Now.AddDays(5)), TemperatureC = -5, Summary = "Freezing" }
    }.AsQueryable();

    // Apply the filters, sorting, and pagination from the search body.
    var results = forecasts.ApplyFilters(searchBody.Filtering);
    results = results.ApplySorting(searchBody.Sorting);
    results = results.ApplyPagination(searchBody.Pagination);

    return results.ToList();
}
```

You now have filtering, sorting, and pagination.

## Request Body Structure

Wondering how to structure the JSON in the request object? Most of it is based on data you can pull from a MUI DataGrid. Here's an example.
```json
{
  "filtering": {
    "field": "summary",
    "operator": "contains",
    "value": "o"
  },
  "sorting": [
    {
      "field": "temperatureC",
      "sort": "asc"
    }
  ],
  "pagination": {
    "page": 0,
    "pageSize": 4
  }
}
```

You can also use a group of filters instead of just a single one, combined with a logic operator.

```json
{
  "filtering": {
    "logicOperator": "or",
    "items": [
      {
        "field": "summary",
        "operator": "contains",
        "value": "o"
      },
      {
        "field": "temperatureC",
        "operator": ">=",
        "value": 20
      }
  ]},
  "sorting": [
    {
      "field": "temperatureC",
      "sort": "asc"
    }
  ],
  "pagination": {
    "page": 0,
    "pageSize": 4
  }
}
```

Each filter item inside a filter group can also be a filter group.