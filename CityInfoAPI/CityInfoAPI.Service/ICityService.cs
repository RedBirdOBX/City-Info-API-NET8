using CityInfoAPI.Dtos.Models;

namespace CityInfoAPI.Service
{
    public interface ICityService
    {
        // cities
        Task<IEnumerable<CityWithoutPointsOfInterestDto>> GetCitiesAsync(string name, string search, int pageNumber, int pageSize);

        Task<bool> CityExistsAsync(Guid cityGuid);

        Task<CityDto?> GetCityAsync(Guid cityGuid, bool includePointsOfInterest);

        Task<CityWithoutPointsOfInterestDto?> GetCityWithoutPointsOfInterestAsync(Guid cityGuid, bool includePointsOfInterest);

        Task<CityWithoutPointsOfInterestDto?> CreateCityAsync(CityCreateDto request);

        Task<CityDto?> UpdateCityAsync(CityUpdateDto request, Guid cityGuid);

        Task<bool> DeleteCityAsync(Guid cityGuid);


        // global
        Task<bool> SaveChangesAsync();
    }
}