using CityInfoAPI.Dtos.RequestModels;


namespace CityInfoAPI.Test.Tests
{
    public class CityRequestParametersTests
    {
        [Fact]
        [Trait("Category","Request Parameters Tests")]
        public void ObjectInvoked_TypeNewedUp_ObjectHasCorrectDefaultValues()
        {
            var parameters = new CityRequestParameters();
            Assert.True(parameters.IncludePointsOfInterest);
            Assert.True(parameters.PageNumber == 1);
            Assert.True(parameters.PageSize == 25);
            Assert.True(parameters.Name == null);
            Assert.True(parameters.Search == null);
        }
    }
}
