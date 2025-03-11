using CityInfoAPI.Data.Entities;

namespace CityInfoAPI.Data.Repositories
{
    public interface ICityRepository
    {
        Task<IEnumerable<City>> GetCitiesUnsortedAsync();

        Task<IEnumerable<City>> GetCitiesAsync(string? name, string? search, int pageNumber, int pageSize);

        Task<City?> GetCityAsync(Guid cityId, bool includePointsOfInterest);

        Task<int> GetCitiesCountAsync();

        Task<City?> CreateCityAsync(City newCity);

        Task<bool> CityNameMatchesCityIdAsync(string? name, Guid cityGuid);

        Task<bool> CityExistsAsync(Guid cityGuid);

        Task DeleteCityAsync(Guid cityGuid);

        Task<bool> SaveChangesAsync();
    }
}
