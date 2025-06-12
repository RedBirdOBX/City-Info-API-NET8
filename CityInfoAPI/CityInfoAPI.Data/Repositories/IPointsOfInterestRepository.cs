using CityInfoAPI.Data.Entities;

namespace CityInfoAPI.Data.Repositories;

public interface IPointsOfInterestRepository
{
    Task<IEnumerable<PointOfInterest>> GetPointsOfInterestGenericAsync();

    Task<IEnumerable<PointOfInterest>> GetPointsOfInterestAsync(string? name, string? search);

    Task<IEnumerable<PointOfInterest>> GetPointsOfInterestForCityAsync(Guid cityGuid);

    Task<PointOfInterest?> GetPointOfInterestAsync(Guid pointGuid);

    Task<int> CountPointOfInterestForCityAsync(Guid cityGuid);

    Task<PointOfInterest?> CreatePointOfInterestAsync(PointOfInterest newPointOfInterest);

    Task<bool> PointOfInterestExistsAsync(Guid pointGuid);

    Task DeletePointOfInterestAsync(Guid pointGuid);

    Task<bool> SaveChangesAsync();
}
