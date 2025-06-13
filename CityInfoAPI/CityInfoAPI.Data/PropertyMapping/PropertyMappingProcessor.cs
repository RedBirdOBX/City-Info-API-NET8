using CityInfoAPI.Data.Entities;
using CityInfoAPI.Dtos;

namespace CityInfoAPI.Data.PropertyMapping;

/// <summary>
/// Goal is to create a reusable service which allows our repositories to sort by
/// and given column name (Name, Description, etc.). Since DTO don't always map directly
/// to the entity, we will use a dictionary to map the property names of the DTOs.
/// The source is the incoming; the alias or computed property.
/// The destination is the true source of data; the entity.
/// </summary>
public class PropertyMappingProcessor : IPropertyMappingProcessor
{
    // TO DO - populate this some other way like constructor
    private readonly Dictionary<string, PropertyMappingValue> _cityPropertyMapping = new(StringComparer.OrdinalIgnoreCase)
    {
        // Source (dto) / Destination (entity)
        // name of destination property (dto) and list of source properties (entities)
        {
            "CityGuid",
            new PropertyMappingValue(new List<string> { "CityGuid" })
        },
        {
            "Name",
            new PropertyMappingValue(new List<string> { "Name" })
        },
        {
            "Description",
            new PropertyMappingValue(new List<string> { "Description" })
        },
        { "CreatedOn",
            new PropertyMappingValue(new List<string> { "CreatedOn" }, true)
        },
        {
            "State",
            new PropertyMappingValue(new List<string> { "State" })
        },
        {
            "CityCode",
            new PropertyMappingValue(new List < string > { "CityGuid" })
        }
        //{ "Name", new PropertyMappingValue(new[] { "FirstName", "LastName" }) }
        //{ "Age", new(new[] { "DateOfBirth" }, true) }
    };
    private readonly IList<IPropertyMapping> _propertyMappings = new List<IPropertyMapping>();

    /// <summary>
    /// Constructor
    /// </summary>
    public PropertyMappingProcessor()
    {
        _propertyMappings.Add(new PropertyMapping<CityDto, City>(_cityPropertyMapping));
    }


    // This should return us the correctly registered mapping from a certain source type to a
    // certain destination type. So we search for that in our PropertyMappings list.
    // Through this, we will be able to ask for a mapping from a source type (CityDto)
    // to a destination type (City entity).
    // The source is the incoming; the alias or computed property.
    // The destination is the true source of data; the entity.
    public Dictionary<string, PropertyMappingValue> GetPropertyMapping<TSource, TDestination>()
    {
        // get matching mapping
        // pass in the type we're looking for. If found, return dictionary of mappings.
        var matchingMapping = _propertyMappings.OfType<PropertyMapping<TSource, TDestination>>();

        if (matchingMapping.Count() == 1)
        {
            return matchingMapping.First().MappingDictionary;
        }
        else
        {
            throw new Exception($"Cannot find property mapping for {typeof(TSource)} to {typeof(TDestination)}");
        }
    }

    public bool ValidMappingExistsFor<TSource, TDestination>(string fields)
    {
        var propertyMapping = GetPropertyMapping<TSource, TDestination>();

        if (string.IsNullOrWhiteSpace(fields))
        {
            // no fields specified, so valid
            return true; 
        }

        var fieldsAfterSplit = fields.Split(',');

        foreach (var field in fieldsAfterSplit)
        {
            var trimmedField = field.Trim();

            var indexOfFirstSpace = trimmedField.IndexOf(" ");
            var propertyName = indexOfFirstSpace == -1 
                                ? trimmedField 
                                : trimmedField.Remove(indexOfFirstSpace);

            // find the matching property
            if (!propertyMapping.ContainsKey(propertyName)) 
            { 
                return false;
            }
        }
        return true;
    }
}
