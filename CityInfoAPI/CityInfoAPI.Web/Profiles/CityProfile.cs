using AutoMapper;
using CityInfoAPI.Data.Entities;
using CityInfoAPI.Dtos.Models;

namespace CityInfoAPI.Web.Profiles
{
    public class CityProfile : Profile
    {
        public CityProfile()
        {
            // source, destination
            CreateMap<City, CityWithoutPointsOfInterestDto>();
            CreateMap<City, CityDto>();
            CreateMap<City, CityCreateDto>();
            CreateMap<CityCreateDto, City>();
            CreateMap<CityUpdateDto, City>();
            CreateMap<City, CityUpdateDto>();
        }
    }
}
