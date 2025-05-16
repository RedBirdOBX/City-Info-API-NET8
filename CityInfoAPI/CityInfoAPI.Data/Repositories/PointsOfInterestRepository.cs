using CityInfoAPI.Data.DbContents;
using CityInfoAPI.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace CityInfoAPI.Data.Repositories
{
    public class PointsOfInterestRepository : IPointsOfInterestRepository
    {
        private readonly CityInfoDbContext _dbContext;
        private readonly ICityRepository _cityRepo;
        private readonly ILogger<PointsOfInterestRepository> _logger;

        public PointsOfInterestRepository(CityInfoDbContext dbContext, ICityRepository cityRepo, ILogger<PointsOfInterestRepository> logger)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException();
            _cityRepo = cityRepo;
            _logger = logger ?? throw new ArgumentNullException();
        }

        public async Task<IEnumerable<PointOfInterest>> GetPointsOfInterestGenericAsync()
        {
            try
            {
                var pointsOfInterest = await _dbContext.PointsOfInterest
                                                        .Include(p => p.City)
                                                        .AsNoTracking()
                                                        .OrderBy(p => p.Name)
                                                        .ToListAsync();
                return pointsOfInterest;
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred while getting points of interest. {ex}");
                throw;
            }
        }

        public async Task<IEnumerable<PointOfInterest>> GetPointsOfInterestAsync(string? name, string? search)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(name) && string.IsNullOrEmpty(search))
                {
                    return await GetPointsOfInterestGenericAsync();
                }

                // IQueryable uses deferred execution
                // the query variable itself never holds the query results and only
                // stores the query commands. Execution of the query is deferred until
                // the query variable is iterated over.
                var pointsOfInterest = _dbContext.PointsOfInterest as IQueryable<PointOfInterest>;

                // constructing the query,,,
                if (!name.IsNullOrEmpty())
                {
                    name = name.Trim().ToLower();
                    pointsOfInterest = pointsOfInterest.Where(p => p.Name.ToLower() == name);
                }

                if (!search.IsNullOrEmpty())
                {
                    search = search.Trim().ToLower();
                    pointsOfInterest = pointsOfInterest.Where(p => p.Name.ToLower()
                                                        .Contains(search) || (p.Description != null && p.Description.ToLower().Contains(search)));
                }

                // query is sent
                var results = await pointsOfInterest.AsNoTracking()
                                                    .OrderBy(p => p.Name)
                                                    .Include(p => p.City)
                                                    .ToListAsync();
                return results;
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred while getting searched/filtered points of interest. {ex}");
                throw;
            }
        }

        public async Task<IEnumerable<PointOfInterest>> GetPointsOfInterestForCityAsync(Guid cityGuid)
        {
            try
            {
                var pointsOfInterest = await _dbContext.PointsOfInterest
                                                        .Include(p => p.City)
                                                        .AsNoTracking()
                                                        .Where(p => p.CityGuid == cityGuid)
                                                        .ToListAsync();
                return pointsOfInterest;
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred while getting points of interest for city {cityGuid}. {ex}");
                throw;
            }
        }

        public async Task<PointOfInterest?> GetPointOfInterestAsync(Guid pointGuid)
        {
            try
            {
                var pointofInterest = await _dbContext.PointsOfInterest
                                                        .Where(p => p.PointGuid == pointGuid)
                                                        .Include(p => p.City)
                                                        .AsNoTracking()
                                                        .FirstOrDefaultAsync();
                return pointofInterest;
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred while getting point of interest: {pointGuid}. {ex}");
                throw;
            }
        }

        public async Task<int> CountPointOfInterestForCityAsync(Guid cityGuid)
        {
            try
            {
                return await _dbContext.PointsOfInterest.CountAsync(p => p.CityGuid == cityGuid);
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred while counting points of interest for city: {cityGuid}. {ex}");
                throw;
            }
        }

        public async Task<PointOfInterest?> CreatePointOfInterestAsync(PointOfInterest newPointOfInterest)
        {
            try
            {
                var city = await _cityRepo.GetCityAsync(newPointOfInterest.CityGuid, false);

                // we need to add the city id to create relationship
                newPointOfInterest.CityId = city.Id;
                if (city != null)
                {
                    city.PointsOfInterest.Add(newPointOfInterest);
                }

                return newPointOfInterest;
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred while creating point of interest. {ex}");
                throw;
            }
        }

        public async Task<bool> PointOfInterestExistsAsync(Guid pointGuid)
        {
            try
            {
                return await _dbContext.PointsOfInterest.AnyAsync(p => p.PointGuid == pointGuid);
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred while checking for point of interest: {pointGuid}. {ex}");
                throw;
            }
        }

        public async Task DeletePointOfInterestAsync(Guid pointGuid)
        {
            try
            {
                var pointOfInterest = await GetPointOfInterestAsync(pointGuid);
                _dbContext.PointsOfInterest.Remove(pointOfInterest);
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred while deleting point of interest: {pointGuid}. {ex}");
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
                _logger.LogError($"An error occurred while saving changes on point of interest. {ex}");
                throw;
            }
        }
    }
}
