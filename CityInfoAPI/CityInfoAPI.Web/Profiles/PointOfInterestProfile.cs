using AutoMapper;
using CityInfoAPI.Data.Entities;
using CityInfoAPI.Dtos;


namespace CityInfoAPI.Web.Profiles
{
    #pragma warning disable CS1591
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
    #pragma warning restore CS1591
}
