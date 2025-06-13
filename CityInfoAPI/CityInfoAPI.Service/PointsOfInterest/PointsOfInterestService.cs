using AutoMapper;
using CityInfoAPI.Data.Entities;
using CityInfoAPI.Data.Repositories;
using CityInfoAPI.Dtos;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace CityInfoAPI.Service;

public class PointsOfInterestService : IPointsOfInterestService
{
    private readonly IPointsOfInterestRepository _repo;
    private readonly IMapper _mapper;
    private readonly ILogger<PointsOfInterestService> _logger;

    public PointsOfInterestService(IPointsOfInterestRepository repo, IMapper mapper, ILogger<PointsOfInterestService> logger)
    {
        _repo = repo;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<IEnumerable<PointOfInterestDto>> GetPointsOfInterestAsync(string name, string search)
    {
        try
        {
            var pointsOfInterest = await _repo.GetPointsOfInterestAsync(name, search);
            var results = _mapper.Map<IEnumerable<PointOfInterestDto>>(pointsOfInterest);
            return results;
        }
        catch (Exception ex)
        {
            _logger.LogError($"An error occurred while getting points of interest. {ex}");
            throw;
        }
    }

    public async Task<IEnumerable<PointOfInterestDto>> GetPointsOfInterestForCityAsync(Guid cityGuid)
    {
        try
        {
            var pointsOfInterest = await _repo.GetPointsOfInterestForCityAsync(cityGuid);
            var results = _mapper.Map<IEnumerable<PointOfInterestDto>>(pointsOfInterest);
            return results;
        }
        catch (Exception ex)
        {
            _logger.LogError($"An error occurred while getting points of interest for city {cityGuid}. {ex}");
            throw;
        }
    }

    public async Task<PointOfInterestDto?> GetPointOfInterestAsync(Guid pointGuid)
    {
        try
        {
            var pointOfInterest = await _repo.GetPointOfInterestAsync(pointGuid);
            var result = _mapper.Map<PointOfInterestDto>(pointOfInterest);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError($"An error occurred while getting the point of interest: {pointGuid}. {ex}");
            throw;
        }
    }

    public async Task<int> CountPointsOfInterestForCityAsync(Guid cityGuid)
    {
        try
        {
            var count = await _repo.CountPointOfInterestForCityAsync(cityGuid);
            return count;
        }
        catch (Exception ex)
        {
            _logger.LogError($"An error occurred while counting the point of interests for city: {cityGuid}. {ex}");
            throw;
        }
    }

    public async Task<PointOfInterestDto?> CreatePointOfInterestAsync(Guid cityGuid, PointOfInterestCreateDto request)
    {
        try
        {
            // map the request
            var newPointRequest = _mapper.Map<PointOfInterest>(request);

            // create new call
            var newPointResults = await _repo.CreatePointOfInterestAsync(newPointRequest);

            // save it
            var success = await SaveChangesAsync();

            if (!success)
            {
                _logger.LogError("An error occurred while point of interest.");
                return null;
            }

            // map the results
            var results = _mapper.Map<PointOfInterestDto>(newPointResults);
            return results;
        }
        catch (Exception ex)
        {
            _logger.LogError($"An error occurred while creating point of interest for city {cityGuid}. Request: {JsonConvert.SerializeObject(request)}. Error: {ex}");
            throw;
        }
    }

    public async Task<PointOfInterestUpdateDto?> UpdatePointOfInterestAsync(Guid cityGuid, Guid pointGuid, PointOfInterestUpdateDto request)
    {
        try
        {
            var existingPointOfInterest = await _repo.GetPointOfInterestAsync(pointGuid);

            // map the request - override the values of the destination object w/ source
            _mapper.Map(request, existingPointOfInterest);

            var success = await SaveChangesAsync();
            if (!success)
            {
                return null;
            }

            // map new entity to dto and return it
            PointOfInterestUpdateDto updatedPointOfInterestDto = _mapper.Map<PointOfInterestUpdateDto>(existingPointOfInterest);

            return updatedPointOfInterestDto;
        }
        catch (Exception ex)
        {
            _logger.LogError($"An error occurred while updating point of interest for city {cityGuid}. Request: {JsonConvert.SerializeObject(request)}. Error: {ex}");
            throw;
        }
    }

    public async Task<bool> DeletePointOfInterestAsync(Guid pointGuid)
    {
        try
        {
            // delete the point of interest
            await _repo.DeletePointOfInterestAsync(pointGuid);
            var results = await SaveChangesAsync();
            return results;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error occurred when deleting point of interest {pointGuid}. Error: {ex}");
            throw;
        }
    }

    public async Task<bool> PointOfInterestExistsAsync(Guid pointGuid)
    {
        try
        {
            return await _repo.PointOfInterestExistsAsync(pointGuid);
        }
        catch (Exception ex)
        {
            _logger.LogError($"An error occurred while checking if point of interest exists. {ex}");
            throw;
        }
    }

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
