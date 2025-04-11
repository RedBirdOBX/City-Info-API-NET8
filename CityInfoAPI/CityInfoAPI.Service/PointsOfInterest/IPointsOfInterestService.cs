using CityInfoAPI.Dtos.Models;

namespace CityInfoAPI.Service
{
    public interface IPointsOfInterestService
    {
        Task<IEnumerable<PointOfInterestDto>> GetPointsOfInterestAsync(string name, string search);

        Task<IEnumerable<PointOfInterestDto>> GetPointsOfInterestForCityAsync(Guid cityGuid);

        Task<int> CountPointsOfInterestForCityAsync(Guid cityGuid);

        Task<bool> PointOfInterestExistsAsync(Guid pointGuid);

        Task<PointOfInterestDto?> GetPointOfInterestAsync(Guid pointGuid);

        Task<PointOfInterestDto?> CreatePointOfInterestAsync(Guid cityGuid, PointOfInterestCreateDto request);

        Task<PointOfInterestUpdateDto?> UpdatePointOfInterestAsync(Guid cityGuid, Guid pointGuid, PointOfInterestUpdateDto request);

        Task<bool> DeletePointOfInterestAsync(Guid pointGuid);

        Task<bool> SaveChangesAsync();
    }
}