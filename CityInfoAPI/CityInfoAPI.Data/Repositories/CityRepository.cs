using CityInfoAPI.Data.DbContents;
using CityInfoAPI.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace CityInfoAPI.Data.Repositories
{
    public class CityRepository : ICityRepository
    {
        private readonly CityInfoDbContext _dbContext;
        private readonly ILogger<CityRepository> _logger;

        public CityRepository(CityInfoDbContext dbContext, ILogger<CityRepository> logger)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException();
            _logger = logger ?? throw new ArgumentNullException();
        }

        public async Task<IEnumerable<City>> GetCitiesUnsortedAsync()
        {
            try
            {
                return await _dbContext.Cities.ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred while getting cities. {ex}");
                throw;
            }
        }

        public async Task<IEnumerable<City>> GetCitiesAsync(string? name, string? search, int pageNumber, int pageSize)
        {
            try
            {
                // IQueryable uses deferred execution
                // the query variable itself never holds the query results and only
                // stores the query commands. Execution of the query is deferred until
                // the query variable is iterated over.
                var cities = _dbContext.Cities as IQueryable<City>;

                // constructing the query,,,
                if (!name.IsNullOrEmpty())
                {
                    name = name.Trim().ToLower();
                    cities = cities.Where(c => c.Name.ToLower() == name);
                }

                if (!search.IsNullOrEmpty())
                {
                    search = search.Trim().ToLower();
                    cities = cities.Where(c => c.Name.ToLower()
                                    .Contains(search) || (c.Description != null && c.Description.ToLower().Contains(search)));
                }

                DateTime start = DateTime.Now;
                // query is sent
                var results = await cities.OrderBy(c => c.Name)
                                            .Skip(pageSize * (pageNumber - 1))
                                            .Take(pageSize)
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
                                            .Where(c => c.CityGuid == cityGuid)
                                            .FirstOrDefaultAsync();
                }
                return await _dbContext.Cities
                                            .Where(c => c.CityGuid == cityGuid)
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
}
