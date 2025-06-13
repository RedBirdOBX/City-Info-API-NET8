using CityInfoAPI.Data.Entities;
using CityInfoAPI.Dtos.RequestModels;


namespace CityInfoAPI.Data.Repositories;

public interface ICityRepository
{
    Task<int> CountCitiesAsync(CityRequestParameters requestParams);

    Task<IEnumerable<City>> GetCitiesAsync(CityRequestParameters requestParams);

    Task<City?> GetCityAsync(Guid cityId, bool includePointsOfInterest);

    Task<int> GetCitiesCountAsync();

    Task<City?> CreateCityAsync(City newCity);

    Task<bool> CityNameMatchesCityIdAsync(string? name, Guid cityGuid);

    Task<bool> CityExistsAsync(Guid cityGuid);

    Task DeleteCityAsync(Guid cityGuid);

    Task<bool> SaveChangesAsync();
}
