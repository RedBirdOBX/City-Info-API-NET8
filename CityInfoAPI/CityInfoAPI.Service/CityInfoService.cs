using AutoMapper;
using CityInfoAPI.Data.Repositories;
using CityInfoAPI.Dtos.Models;
using Microsoft.Extensions.Logging;

namespace CityInfoAPI.Service
{
    public class CityInfoService : ICityInfoService
    {
        private readonly ICityInfoRepository _repo;
        private readonly IMapper _mapper;
        private readonly ILogger<CityInfoService> _logger;

        public CityInfoService(ICityInfoRepository repo, IMapper mapper, ILogger<CityInfoService> logger)
        {
            _repo = repo;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IEnumerable<CityWithoutPointsOfInterestDto>> GetCitiesAsync(string name, string search, int pageNumber, int pageSize)
        {
            try
            {
                var cities = await _repo.GetCitiesAsync(name, search, pageNumber, pageSize);
                var results = _mapper.Map<IEnumerable<CityWithoutPointsOfInterestDto>>(cities);
                return results;
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred while getting cities. {ex}");
                throw;
            }
        }
    }
}
