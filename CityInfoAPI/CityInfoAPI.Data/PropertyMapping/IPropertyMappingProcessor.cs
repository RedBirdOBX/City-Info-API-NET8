namespace CityInfoAPI.Data.PropertyMapping;

public interface IPropertyMappingProcessor
{
    Dictionary<string, PropertyMappingValue> GetPropertyMapping<TSource, TDestination>();
}