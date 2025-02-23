using CityInfoAPI.Data.DbContents;
using CityInfoAPI.Data.Entities;
using Microsoft.EntityFrameworkCore;

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

        public async Task<IEnumerable<City>> GetCitiesAsync(string? name)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(name))
                {
                    return await GetCitiesAsync();
                }

                name = name.Trim().ToLower();

                //return await _dbContext.Cities.Where(c => c.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase))
                //                                .OrderBy(c => c.Name)
                //                                .ToListAsync();

                //string str1 = "Hello";
                //string str2 = "hello";

                //bool areEqual = str1.Equals(str2, StringComparison.OrdinalIgnoreCase);
                //Console.WriteLine(areEqual); // Output: True
                //bool isBillPostalCodeSame = leadAdd1PostalCode.Equals(custBillPostalCode, StringComparison.InvariantCultureIgnoreCase);

                return await _dbContext.Cities.Where(c => c.Name.ToLower() == name)
                                                .OrderBy(c => c.Name)
                                                .ToListAsync();
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
