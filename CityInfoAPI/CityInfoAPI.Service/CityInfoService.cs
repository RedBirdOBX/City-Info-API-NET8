using AutoMapper;
using CityInfoAPI.Data.Entities;
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

        // cities
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

        public async Task<bool> CityExistsAsync(Guid cityGuid)
        {
            try
            {
                return await _repo.CityExistsAsync(cityGuid);
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred while checking if city exists. {ex}");
                throw;
            }
        }

        public async Task<CityDto?> GetCityAsync(Guid cityGuid, bool includePointsOfInterest)
        {
            try
            {
                var city = await _repo.GetCityByCityIdAsync(cityGuid, includePointsOfInterest);
                var results = _mapper.Map<CityDto>(city);
                return results;
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred while fetching city. {ex}");
                throw;
            }
        }

        public async Task<CityWithoutPointsOfInterestDto?> GetCityWithoutPointsOfInterestAsync(Guid cityGuid, bool includePointsOfInterest)
        {
            try
            {
                var city = await _repo.GetCityByCityIdAsync(cityGuid, includePointsOfInterest);
                var results = _mapper.Map<CityWithoutPointsOfInterestDto>(city);
                return results;
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred while fetching city without points of interest. {ex}");
                throw;
            }
        }

        public async Task<CityDto?> CreateCityAsync(CityCreateDto newCityRequest)
        {
            try
            {
                var newCityEntity = _mapper.Map<City>(newCityRequest);

                // add it to memory.
                await _repo.CreateCityAsync(newCityEntity);

                // save it
                bool success = await _repo.SaveChangesAsync();

                if (!success)
                {
                    return null;
                }

                // map new entity to dto and return it
                CityDto newCityDto = _mapper.Map<CityDto>(newCityEntity);

                return newCityDto;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occurred when creating a city: {ex}");
                throw;
            }
        }

        public async Task<CityDto?> UpdateCityAsync(CityUpdateDto updateCityRequest, Guid cityGuid)
        {
            try
            {
                var city = await _repo.GetCityByCityIdAsync(cityGuid, false);

                // map the request - override the values of the destination object w/ source
                _mapper.Map(updateCityRequest, city);

                var success = await _repo.SaveChangesAsync();

                if (!success)
                {
                    return null;
                }

                // map new entity to dto and return it
                CityDto updatedCityDto = _mapper.Map<CityDto>(updateCityRequest);

                return updatedCityDto;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occurred when creating a city: {ex}");
                throw;
            }
        }

        public async Task<bool> DeleteCityAsync(Guid cityGuid)
        {
            try
            {
                // delete the city
                await _repo.DeleteCityAsync(cityGuid);
                var results = await _repo.SaveChangesAsync();
                return results;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occurred when creating a city: {ex}");
                throw;
            }
        }

        // global
        public async Task<bool> SaveChangesAsync()
        {
            try
            {
                return await _repo.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred while saving changes. {ex}");
                throw;
            }
        }
    }
}
