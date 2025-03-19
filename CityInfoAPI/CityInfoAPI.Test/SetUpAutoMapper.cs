using AutoMapper;
using CityInfoAPI.Web.Profiles;

namespace CityInfoAPI.Test
{
    public static class SetUpAutoMapper
    {
        public static IMapper SetUp()
        {
            var config = new MapperConfiguration(opts =>
            {
                opts.AddProfile(new CityProfile());
                opts.AddProfile(new PointOfInterestProfile());
            });

            return config.CreateMapper();
        }
    }
}
