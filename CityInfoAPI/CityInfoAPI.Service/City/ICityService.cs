using CityInfoAPI.Dtos;
using CityInfoAPI.Dtos.RequestModels;


namespace CityInfoAPI.Service
{
    public interface ICityService
    {
        Task<int> CountCitiesAsync(CityRequestParameters requestParams);

        Task<IEnumerable<CityDto>> GetCitiesAsync(CityRequestParameters requestParams);

        Task<bool> CityExistsAsync(Guid cityGuid);

        Task<CityDto?> GetCityAsync(Guid cityGuid, bool includePointsOfInterest);

        Task<CityDto?> CreateCityAsync(CityCreateDto request);

        Task<CityDto?> UpdateCityAsync(CityUpdateDto request, Guid cityGuid);

        Task<bool> DeleteCityAsync(Guid cityGuid);

        Task<bool> SaveChangesAsync();
    }
}