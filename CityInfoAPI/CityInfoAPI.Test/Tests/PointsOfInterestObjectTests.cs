using CityInfoAPI.Data.Entities;
using CityInfoAPI.Dtos.Models;

namespace CityInfoAPI.Test.Tests
{
    public class PointsOfInterestObjectTests
    {
        [Fact]
        [Trait("Category","Points Of Interest Object Tests")]
        public void PointsOfInterestEntityObjectInvoked_TypeNewedUp_ObjectHasCorrectDefaultValues()
        {
            var poi = new PointOfInterest();
            Assert.IsType<Guid>(poi.PointGuid);
            Assert.IsType<Guid>(poi.CityGuid);
            Assert.True(poi.CreatedOn > DateTime.Now.AddSeconds(-5));
        }

        [Fact]
        [Trait("Category","Points Of Interest Object Tests")]
        public void PointsOfInterestDtoObjectInvoked_TypeNewedUp_ObjectHasCorrectDefaultValues()
        {
            var poi = new PointOfInterestDto();
            Assert.IsType<Guid>(poi.PointGuid);
            Assert.IsType<Guid>(poi.CityGuid);
            Assert.True(poi.CreatedOn > DateTime.Now.AddSeconds(-5));
        }
    }
}
