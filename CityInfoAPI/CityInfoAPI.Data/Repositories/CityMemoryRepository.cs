using CityInfoAPI.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;


namespace CityInfoAPI.Data.Repositories
{
    #pragma warning disable CS8618

    public class CityMemoryRepository : ICityRepository
    {
        public List<City> Cities { get; set; }
        public List<PointOfInterest> PointsOfInterest { get; set; }

        public CityMemoryRepository()
        {
            Cities = new CityInfoTestEntityData().Cities;
            PointsOfInterest = new CityInfoTestEntityData().PointsOfInterest;
        }

        public async Task<IEnumerable<City>> GetCitiesUnsortedAsync()
        {
            var results = Cities.OrderBy(c => c.Name).ToList();
            return results;
        }

        public async Task<IEnumerable<City>> GetCitiesAsync(string? name, string? search, int pageNumber, int pageSize)
        {
            try
            {
                // IQueryable uses deferred execution
                // the query variable itself never holds the query results and only
                // stores the query commands. Execution of the query is deferred until
                // the query variable is iterated over.
                var cities = Cities as IQueryable<City>;

                // constructing the query...
                if (!name.IsNullOrEmpty())
                {
                    name = name.Trim().ToLower();
                    cities = cities.Where(c => c.Name.ToLower() == name);
                }

                if (!search.IsNullOrEmpty())
                {
                    search = search.Trim().ToLower();
                    cities = cities.Where(c => c.Name.ToLower()
                                    .Contains(search) || c.Description != null && c.Description.ToLower().Contains(search));
                }

                // query is sent
                var results = await cities.OrderBy(c => c.Name)
                                            .Skip(pageSize * (pageNumber - 1))
                                            .Take(pageSize)
                                            .ToListAsync();
                return results;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<City?> GetCityAsync(Guid cityGuid, bool includePointsOfInterest)
        {
            try
            {
                var city = Cities.Where(c => c.CityGuid == cityGuid).FirstOrDefault();

                if (includePointsOfInterest)
                {
                    foreach (var poi in PointsOfInterest)
                    {
                        if (poi.CityGuid == city.CityGuid)
                        {
                            city.PointsOfInterest.Add(poi);
                        }
                    }
                }
                return city;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<int> GetCitiesCountAsync()
        {
            try
            {
                return Cities.Count();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<City?> CreateCityAsync(City newCity)
        {
            try
            {
                Cities.Add(newCity);
                return newCity;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<bool> CityNameMatchesCityIdAsync(string? name, Guid cityGuid)
        {
            try
            {
                return Cities.Any(c => c.CityGuid == cityGuid && c.Name.ToLower() == name.ToLower());
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<bool> CityExistsAsync(Guid cityGuid)
        {
            try
            {
                return Cities.Any(c => c.CityGuid == cityGuid);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task DeleteCityAsync(Guid cityGuid)
        {
            try
            {
                var city = GetCityAsync(cityGuid, false).Result;
                Cities.Remove(city);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<bool> SaveChangesAsync()
        {
            return true;
        }
    }

    #pragma warning restore CS8618
}
