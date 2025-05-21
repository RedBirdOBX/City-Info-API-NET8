using CityInfoAPI.Dtos.Models;

namespace CityInfoAPI.Service
{
    public interface IStateService
    {
        Task<IEnumerable<StateDto>> GetStatesAsync();

        Task<StateDto?> GetStateAsync(string stateCode);

        Task<bool> StateExistsAsync(string stateCode);
    }
}