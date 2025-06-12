using CityInfoAPI.Data.DbContents;
using CityInfoAPI.Data.Entities;
using CityInfoAPI.Data.Extensions;
using CityInfoAPI.Data.PropertyMapping;
using CityInfoAPI.Dtos;
using CityInfoAPI.Dtos.RequestModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;


namespace CityInfoAPI.Data.Repositories;

public class CityRepository : ICityRepository
{
    private readonly CityInfoDbContext _dbContext;
    private readonly IPropertyMappingProcessor _propMapper;
    private readonly ILogger<CityRepository> _logger;

    public CityRepository(CityInfoDbContext dbContext, IPropertyMappingProcessor propMapper, ILogger<CityRepository> logger)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException();
        _propMapper = propMapper ?? throw new ArgumentNullException();
        _logger = logger ?? throw new ArgumentNullException();
    }

    public async Task<int> CountCitiesAsync(CityRequestParameters requestParams)
    {
        try
        {
            var cities = _dbContext.Cities as IQueryable<City>;
            int count = 0;

            // constructing the query...
            if (!string.IsNullOrEmpty(requestParams?.Name))
            {
                cities = cities.Where(c => c.Name.ToLower() == requestParams.Name.ToLower());
            }

            if (!string.IsNullOrEmpty(requestParams?.Search))
            {
                requestParams.Search = requestParams.Search.Trim().ToLower();
                cities = cities.Where(c => c.Name.Contains(requestParams.Search) || (c.Description != null && c.Description.ToLower().Contains(requestParams.Search)));
            }

            count = await cities.CountAsync();
            return count;
        }
        catch (Exception ex)
        {
            _logger.LogError($"An error occurred while counting cities. {ex}");
            throw;
        }
    }

    public async Task<IEnumerable<City>> GetCitiesAsync(CityRequestParameters requestParams)
    {
        try
        {
            // IQueryable uses deferred execution
            // the query variable itself never holds the query results and only
            // stores the query commands. Execution of the query is deferred until
            // the query variable is iterated over.
            var cities = _dbContext.Cities as IQueryable<City>;

            // constructing the query...
            if (!string.IsNullOrEmpty(requestParams?.Name))
            {
                cities = cities.Where(c => c.Name.ToLower() == requestParams.Name.ToLower());
            }

            if (!string.IsNullOrEmpty(requestParams?.Search))
            {
                requestParams.Search = requestParams.Search.Trim().ToLower();
                cities = cities.Where(c => c.Name.Contains(requestParams.Search) || (c.Description != null && c.Description.ToLower().Contains(requestParams.Search)));
            }

            if (requestParams.IncludePointsOfInterest)
            {
                cities = cities.Include(c => c.PointsOfInterest);
            }

            if (!string.IsNullOrWhiteSpace(requestParams.OrderBy))
            {
                // get the mapping dictionary for the cities
                var citiesPropertyMappingDictionary = _propMapper.GetPropertyMapping<CityDto, City>();

                // using our custom Property Mapping Processor
                cities = cities.ApplySort(requestParams.OrderBy, citiesPropertyMappingDictionary);
            }

            // query is sent
            var results = await cities .Skip(requestParams.PageSize * (requestParams.PageNumber - 1))
                                        .Take(requestParams.PageSize)
                                        .Include(c => c.State)
                                        .AsNoTracking()
                                        .ToListAsync();

            return results;
        }
        catch (Exception ex)
        {
            _logger.LogError($"An error occurred while getting search / filter cities. {ex}");
            throw;
        }
    }

    public async Task<City?> GetCityAsync(Guid cityGuid, bool includePointsOfInterest)
    {
        try
        {
            if (includePointsOfInterest)
            {
                return await _dbContext.Cities
                                        .Include(c => c.PointsOfInterest)
                                        .Include(c => c.State)
                                        .Where(c => c.CityGuid == cityGuid)
                                        .AsNoTracking()
                                        .FirstOrDefaultAsync();
            }
            return await _dbContext.Cities
                                        .Include(c => c.State)
                                        .Where(c => c.CityGuid == cityGuid)
                                        .AsNoTracking()
                                        .FirstOrDefaultAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError($"An error occurred while getting city {cityGuid}. {ex}");
            throw;
        }
    }

    public async Task<int> GetCitiesCountAsync()
    {
        try
        {
            return await _dbContext.Cities.CountAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError($"An error occurred while counting cities. {ex}");
            throw;
        }
    }

    public async Task<City?> CreateCityAsync(City newCity)
    {
        try
        {
            await _dbContext.Cities.AddAsync(newCity);
            return newCity;
        }
        catch (Exception ex)
        {
            _logger.LogError($"An error occurred while creating city. {ex}");
            throw;
        }
    }

    public async Task<bool> CityNameMatchesCityIdAsync(string? name, Guid cityGuid)
    {
        try
        {
            return await _dbContext.Cities.AnyAsync(c => c.CityGuid == cityGuid && c.Name.ToLower() == name.ToLower());
        }
        catch (Exception ex)
        {
            _logger.LogError($"An error occurred while checking city name. {ex}");
            throw;
        }
    }

    public async Task<bool> CityExistsAsync(Guid cityGuid)
    {
        try
        {
            return await _dbContext.Cities.AnyAsync(c => c.CityGuid == cityGuid);
        }
        catch (Exception ex)
        {
            _logger.LogError($"An error occurred while checking if city exists. {ex}");
            throw;
        }
    }

    public async Task DeleteCityAsync(Guid cityGuid)
    {
        try
        {
            var city = await GetCityAsync(cityGuid, false);
            _dbContext.Cities.Remove(city);
        }
        catch (Exception ex)
        {
            _logger.LogError($"An error occurred while deleting city {cityGuid}. {ex}");
            throw;
        }
    }

    public async Task<bool> SaveChangesAsync()
    {
        try
        {
            var changes = await _dbContext.SaveChangesAsync();
            return changes >= 0;
        }
        catch (Exception ex)
        {
            _logger.LogError($"An error occurred while saving city. {ex}");
            throw;
        }
    }
}
