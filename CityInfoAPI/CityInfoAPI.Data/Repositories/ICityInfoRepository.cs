using CityInfoAPI.Data.Entities;

namespace CityInfoAPI.Data.Repositories
{
    public interface ICityInfoRepository
    {
        // cities
        Task<IEnumerable<City>> GetCitiesAsync();

        Task<City?> GetCityByCityIdAsync(Guid cityId, bool includePointsOfInterest);

        Task<City?> CreateCityAsync(City newCity);

        Task<bool> CityExistsAsync(Guid cityGuid);

        Task DeleteCityAsync(Guid cityGuid);

        // points of interest
        Task<IEnumerable<PointOfInterest>> GetPointsOfInterestForCityAsync(Guid cityGuid);

        Task<PointOfInterest?> GetPointOfInterestById(Guid pointGuid);

        Task<PointOfInterest?> CreatePointOfInterestAsync(PointOfInterest newPointOfInterest);

        Task<bool> PointOfInterestExistsAsync(Guid pointGuid);

        Task DeletePointOfInterestAsync(Guid pointGuid);

        // global
        Task<bool> SaveChangesAsync();
    }
}
