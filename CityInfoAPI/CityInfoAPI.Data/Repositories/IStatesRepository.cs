using CityInfoAPI.Data.Entities;

namespace CityInfoAPI.Data.Repositories
{
    public interface IStatesRepository
    {
        Task<IEnumerable<State>> GetStatesAsync();

        Task<State?> GetStateAsync(string stateCode);
    }
}
