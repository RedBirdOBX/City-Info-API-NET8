using CityInfoAPI.Data.Entities;
using CityInfoAPI.Dtos.Models;

namespace CityInfoAPI.Test.Tests
{
    public class CityObjectTests
    {
        [Fact]
        [Trait("Category","City Object Tests")]
        public void CityEntityObjectInvoked_TypeNewedUp_ObjectHasCorrectDefaultValues()
        {
            var city = new City();
            Assert.IsType<Guid>(city.CityGuid);
            Assert.True(city.CreatedOn > DateTime.Now.AddSeconds(-5));
        }

        [Fact]
        [Trait("Category","City Object Tests")]
        public void CityDtoObjectInvoked_TypeNewedUp_ObjectHasCorrectDefaultValues()
        {
            var city = new CityDto();
            Assert.IsType<Guid>(city.CityGuid);
            Assert.True(city.CreatedOn > DateTime.Now.AddSeconds(-5));
        }
    }
}
