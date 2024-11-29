using RestSearch.Filtering;
using RestSearch.Sorting;
using System.Linq.Expressions;

namespace RestSearch;

public static class QueryableExtensions
{
    /// <summary>
    /// Applies the filter(s) to the collection.
    /// </summary>
    /// <remarks>
    /// If the filter is null, no filtering is applied.
    /// </remarks>
    /// <returns>The queryable with filter(s) applied as a Where operation.</returns>
    public static IQueryable<TModel> ApplyFilters<TModel>(this IQueryable<TModel> queryable, Filter<TModel>? filter)
    {
        if (filter == null)
        {
            return queryable;
        }
        var modelParam = Expression.Parameter(typeof(TModel), "model");
        var filterExpression = GetFilterExpression(filter, modelParam);
        var lamda = Expression.Lambda<Func<TModel, bool>>(filterExpression, modelParam);
        return queryable.Where(lamda.Compile()).AsQueryable();
    }

    /// <summary>
    /// Applies the sorting to the collection.
    /// </summary>
    /// <remarks>
    /// The first sort item is applied using OrderBy or OrderByDescending. Subsequent sort items are applied using ThenBy or ThenByDescending.
    /// If there are no valid sort items, no sorting is applied.
    /// </remarks>
    /// <returns>The queryable with the sort items applied.</returns>
    public static IQueryable<TModel> ApplySorting<TModel>(this IQueryable<TModel> queryable, List<SortItem<TModel>> sortItems)
    {
        sortItems = [.. sortItems.Where(
            si => si.Sort != null
            && SearchFieldDefinitionProvider.GetByName<TModel>(si.Field)?.IsSortingAllowed == true)];

        if (sortItems.Count == 0)
        {
            return queryable;
        }
        var modelParam = Expression.Parameter(typeof(TModel), "model");
        var firstOrderByExpression = GetPropertySelectorLamda<TModel>(sortItems[0].Field, modelParam);

        var sortedCollection = sortItems[0].Sort == SortDirection.Ascending
            ? queryable.OrderBy(firstOrderByExpression)
            : queryable.OrderByDescending(firstOrderByExpression);
        foreach (var item in sortItems.Skip(1))
        {
            var orderByExpression = GetPropertySelectorLamda<TModel>(item.Field, modelParam);

            sortedCollection = item.Sort == SortDirection.Ascending
                ? sortedCollection.ThenBy(orderByExpression)
                : sortedCollection.ThenByDescending(orderByExpression);
        }

        return sortedCollection;
    }

    /// <summary>
    /// Applies pagination to the collection.
    /// </summary>
    /// <returns>The queryable with the pagination applied.</returns>
    public static IQueryable<TModel> ApplyPagination<TModel>(this IQueryable<TModel> queryable, Pagination? pagination)
    {
        if (pagination == null || pagination.PageSize <= 0)
        {
            return queryable;
        }
        return queryable.Skip(pagination.Page * pagination.PageSize)
            .Take(pagination.PageSize);
    }

    private static Expression GetFilterExpression<TModel>(Filter<TModel> filter, ParameterExpression modelParam)
    {
        if (filter is FilterGroup<TModel> group)
        {
            var expressions = new List<Expression>();
            foreach (var item in group.Items)
            {
                expressions.Add(GetFilterExpression(item, modelParam));
            }
            Expression? compiled = null;
            foreach (var item in expressions)
            {
                if (compiled == null)
                {
                    compiled = item;
                }
                else
                {
                    if (group.LogicOperator == LogicOperator.And)
                    {
                        compiled = Expression.AndAlso(compiled, item);
                    }
                    else
                    {
                        compiled = Expression.OrElse(compiled, item);
                    }
                }
            }
            return compiled ?? Expression.Empty();
        }
        if (filter is FieldFilter<TModel> fieldFilter)
        {
            var fieldDefinition = SearchFieldDefinitionProvider.GetByName<TModel>(fieldFilter.Field);

            if (fieldDefinition?.FilterOperators?.TryGetValue(fieldFilter.Operator, out var operatorFunc) == true)
            {
                return operatorFunc(fieldFilter, modelParam);
            }
        }
        throw new NotImplementedException();
    }

    private static Expression<Func<TModel, object>> GetPropertySelectorLamda<TModel>(string fieldName, ParameterExpression modelParam)
    {
        var fieldDefinition = SearchFieldDefinitionProvider.GetByName<TModel>(fieldName)!;
        var propertyExpression = Expression.Property(modelParam, fieldDefinition.ClrFieldName);
        var propertySelectorLamda = Expression.Lambda<Func<TModel, object>>(Expression.Convert(propertyExpression, typeof(object)), modelParam);
        return propertySelectorLamda;
    }
}
