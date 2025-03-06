using AutoMapper;
using CityInfoAPI.Data.Entities;
using CityInfoAPI.Dtos.Models;

namespace CityInfoAPI.Web.Profiles
{
    public class PointOfInterestProfile : Profile
    {
        public PointOfInterestProfile()
        {
            // source, destination
            CreateMap<PointOfInterest, PointOfInterestDto>();
            CreateMap<PointOfInterest, PointOfInterestUpdateDto>();
            CreateMap<PointOfInterestDto, PointOfInterestUpdateDto>();
            CreateMap<PointOfInterestCreateDto, PointOfInterest>();
            CreateMap<PointOfInterestUpdateDto, PointOfInterest>();
            CreateMap<PointOfInterestUpdateDto, PointOfInterestDto>();
        }
    }
}
