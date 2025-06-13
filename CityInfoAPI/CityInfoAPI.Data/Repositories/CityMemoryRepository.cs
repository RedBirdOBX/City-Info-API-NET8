using CityInfoAPI.Data.Entities;
using CityInfoAPI.Dtos.RequestModels;
using Microsoft.EntityFrameworkCore;


namespace CityInfoAPI.Data.Repositories;

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

    public async Task<int> CountCitiesAsync(CityRequestParameters requestParams)
    {
        try
        {
            var cities = Cities as IQueryable<City>;
            int count = 0;

            // constructing the query...
            if (!string.IsNullOrEmpty(requestParams?.Name))
            {
                requestParams.Name = requestParams.Name.Trim().ToLower();
                cities = cities.Where(c => c.Name.Equals(requestParams.Name, StringComparison.CurrentCultureIgnoreCase));
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
            throw;
        }
    }

    public async Task<IEnumerable<City>> GetCitiesUnsortedAsync(bool includePointsOfInterest)
    {
        var results = Cities.OrderBy(c => c.Name).ToList();
        if (includePointsOfInterest)
        {
            foreach (var city in results)
            {
                city.PointsOfInterest = PointsOfInterest.Where(poi => poi.CityGuid == city.CityGuid).ToList();
            }
        }
        else
        {
            foreach (var city in results)
            {
                city.PointsOfInterest.Clear();
            }
        }
        return results;
    }

    public async Task<IEnumerable<City>> GetCitiesAsync(CityRequestParameters requestParams)
    {
        try
        {
            // IQueryable uses deferred execution
            // the query variable itself never holds the query results and only
            // stores the query commands. Execution of the query is deferred until
            // the query variable is iterated over.
            var cities = Cities as IQueryable<City>;

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

            // query is sent
            var results = await cities.OrderBy(c => c.Name)
                                        .Skip(requestParams.PageSize * (requestParams.PageNumber - 1))
                                        .Take(requestParams.PageSize)
                                        .Include(c => c.State)
                                        .AsNoTracking()
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

