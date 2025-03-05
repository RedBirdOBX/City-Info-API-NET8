using CityInfoAPI.Dtos.Models;

namespace CityInfoAPI.Service
{
    public interface ICityInfoService
    {
        Task<IEnumerable<CityWithoutPointsOfInterestDto>> GetCitiesAsync(string name, string search, int pageNumber, int pageSize);
    }
}