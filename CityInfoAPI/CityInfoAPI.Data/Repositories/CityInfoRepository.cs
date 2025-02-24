using CityInfoAPI.Data.DbContents;
using CityInfoAPI.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace CityInfoAPI.Data.Repositories
{
    public class CityInfoRepository : ICityInfoRepository
    {
        private readonly CityInfoDbContext _dbContext;

        public CityInfoRepository(CityInfoDbContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException();
        }


        // cities
        public async Task<IEnumerable<City>> GetCitiesAsync()
        {
            try
            {
                return await _dbContext.Cities.ToListAsync();
            }
            catch (Exception ex)
            {
                // logger here
                throw ex;
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

                // query is sent
                var results = await cities.OrderBy(c => c.Name)
                                            .Skip(pageSize * (pageNumber - 1))
                                            .Take(pageSize)
                                            .ToListAsync();
                return results;
            }
            catch (Exception ex)
            {
                // logger here
                throw ex;
            }
        }

        public async Task<City?> GetCityByCityIdAsync(Guid cityGuid, bool includePointsOfInterest)
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
                // logger here
                throw ex;
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
                // logger here
                throw ex;
            }
        }

        public async Task DeleteCityAsync(Guid cityGuid)
        {
            try
            {
                var city = await GetCityByCityIdAsync(cityGuid, false);
                _dbContext.Cities.Remove(city);
            }
            catch (Exception ex)
            {
                // logger here
                throw ex;
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
                // logger here
                throw ex;
            }
        }



        // points of interest
        public async Task<IEnumerable<PointOfInterest>> GetPointsOfInterestAsync()
        {
            try
            {
                var pointsOfInterest = await _dbContext.PointsOfInterest.OrderBy(p => p.Name).ToListAsync();
                return pointsOfInterest;
            }
            catch (Exception ex)
            {
                // logger here
                throw ex;
            }
        }

        public async Task<IEnumerable<PointOfInterest>> GetPointsOfInterestAsync(string? name, string? search)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(name) && string.IsNullOrEmpty(search))
                {
                    return await GetPointsOfInterestAsync();
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
                var results = await pointsOfInterest.OrderBy(p => p.Name).ToListAsync();
                return results;
            }
            catch (Exception ex)
            {
                // logger here
                throw ex;
            }
        }

        public async Task<IEnumerable<PointOfInterest>> GetPointsOfInterestForCityAsync(Guid cityGuid)
        {
            try
            {
                var pointsOfInterest = await _dbContext.PointsOfInterest.Where(p => p.CityGuid == cityGuid).ToListAsync();
                return pointsOfInterest;
            }
            catch (Exception ex)
            {
                // logger here
                throw ex;
            }
        }

        public async Task<PointOfInterest?> GetPointOfInterestById(Guid pointGuid)
        {
            return await _dbContext.PointsOfInterest.Where(p => p.PointGuid == pointGuid).FirstOrDefaultAsync();
        }

        public async Task<PointOfInterest?> CreatePointOfInterestAsync(PointOfInterest newPointOfInterest)
        {
            try
            {
                var city = await GetCityByCityIdAsync(newPointOfInterest.CityGuid, false);

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
                // logger here
                throw ex;
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
                // logger here
                throw ex;
            }
        }

        public async Task DeletePointOfInterestAsync(Guid pointGuid)
        {
            try
            {
                var pointOfInterest = await GetPointOfInterestById(pointGuid);
                _dbContext.PointsOfInterest.Remove(pointOfInterest);
            }
            catch (Exception ex)
            {
                // logger here
                throw ex;
            }
        }


        // global
        public async Task<bool> SaveChangesAsync()
        {
            try
            {
                return await _dbContext.SaveChangesAsync() >= 0;
            }
            catch (Exception ex)
            {
                // logger here
                throw ex;
            }
        }
    }
}
