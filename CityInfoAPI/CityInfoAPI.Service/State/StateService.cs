using AutoMapper;
using CityInfoAPI.Data.Repositories;
using CityInfoAPI.Dtos;
using Microsoft.Extensions.Logging;

namespace CityInfoAPI.Service
{
    public class StateService : IStateService
    {
        private readonly IStatesRepository _repo;
        private readonly IMapper _mapper;
        private readonly ILogger<StateService> _logger;

        public StateService(IStatesRepository repo, IMapper mapper, ILogger<StateService> logger)
        {
            _repo = repo;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IEnumerable<StateDto>> GetStatesAsync()
        {
            try
            {
                var states = await _repo.GetStatesAsync();
                var results = _mapper.Map<IEnumerable<StateDto>>(states);
                return results;
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred while getting states. {ex}");
                throw;
            }
        }

        public async Task<StateDto?> GetStateAsync(string stateCode)
        {
            try
            {
                var state = await _repo.GetStateAsync(stateCode);
                var results = _mapper.Map<StateDto>(state);
                return results;
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred while fetching state. {ex}");
                throw;
            }
        }

        public async Task<bool> StateExistsAsync(string stateCode)
        {
            try
            {
                var state = await _repo.GetStateAsync(stateCode);
                return state != null;
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred while fetching state. {ex}");
                throw;
            }
        }
    }
}
