using CityInfoAPI.Data.Entities;
using CityInfoAPI.Dtos;

namespace CityInfoAPI.Data.PropertyMapping;

// https://app.pluralsight.com/ilx/video-courses/1b57d9e1-fb13-4f6c-9a6d-850dc8e5a78f/3ee9a2b9-1f04-4b93-94cd-fa44860c7b56/3fbbefbb-a060-4429-bb6c-f7d9fc960975


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
}
