﻿namespace CityInfoAPI.Data.PropertyMapping;

public class PropertyMappingValue
{
    public IEnumerable<string> DestinationProperties { get; private set; }

    public bool Revert { get; private set; }

    public PropertyMappingValue(IEnumerable<string> destinationProperties, bool revert = false)
    {
        DestinationProperties = destinationProperties ?? throw new ArgumentNullException(nameof(destinationProperties));
        Revert = revert;
    }
}
