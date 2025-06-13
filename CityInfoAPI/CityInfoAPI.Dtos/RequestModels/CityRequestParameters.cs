namespace CityInfoAPI.Dtos.RequestModels;

/// <summary>
/// obj used to filer / search / page results of cities
/// </summary>
public class CityRequestParameters
{
    /// <summary>
    /// flag to include points of interest in results
    /// </summary>
    public bool IncludePointsOfInterest { get; set; } = true;

    /// <summary>
    /// name filter
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// search string
    /// </summary>
    public string? Search { get; set; }

    /// <summary>
    /// requested page number
    /// </summary>
    public int PageNumber { get; set; } = 1;

    /// <summary>
    /// requested page size
    /// </summary>
    public int PageSize { get; set; } = 25;

    /// <summary>
    /// order by property
    /// </summary>
    public string OrderBy { get; set; } = "Name";
}

