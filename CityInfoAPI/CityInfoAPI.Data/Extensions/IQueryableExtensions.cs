using CityInfoAPI.Data.PropertyMapping;
using System.Linq.Dynamic.Core;


namespace CityInfoAPI.Data.Extensions;

public static class IQueryableExtensions
{
    public static IQueryable<T> ApplySort<T>(this IQueryable<T> source, string orderBy, 
                                            Dictionary<string, PropertyMappingValue> mappingDictionary)
    {

        // must be new.... nice.
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(mappingDictionary);

        var orderByString = string.Empty;
        var orderByAfterSplit = orderBy.Split(',');

        foreach (var orderByClause in orderByAfterSplit)
        {
            string trimmedOrderByClause = orderByClause.Trim();
            bool orderDescending = trimmedOrderByClause.EndsWith(" desc", StringComparison.OrdinalIgnoreCase);
            int indexOfFirstSpace = trimmedOrderByClause.IndexOf(' ');
            string propertyName = indexOfFirstSpace == -1
                                ? trimmedOrderByClause
                                : trimmedOrderByClause.Remove(indexOfFirstSpace);

            // find the matching property
            if (!mappingDictionary.ContainsKey(propertyName))
            {
                throw new ArgumentException($"Key mapping for {propertyName} not found.");
            }

            // get the property mapping value
            var propertyMappingValue = mappingDictionary[propertyName];
            if (propertyMappingValue == null)
            {
                throw new ArgumentException($"propertyMappingValue not found.");
            }

            // revert sort order if necessary
            if (propertyMappingValue.Revert)
            {
                orderDescending = !orderDescending;
            }

            // run thru the property names
            foreach (var destinationProperty in propertyMappingValue.DestinationProperties)
            {
                orderByString = orderByString + (string.IsNullOrWhiteSpace(orderByString)
                                    ?  string.Empty
                                    : ", ") + destinationProperty + (orderDescending ? " descending" : " ascending");
            }
        }
        return source.OrderBy(orderByString);
    }
}
