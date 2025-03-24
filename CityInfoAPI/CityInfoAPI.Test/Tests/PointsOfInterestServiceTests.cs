using AutoMapper;
using CityInfoAPI.Data.Entities;
using CityInfoAPI.Data.Repositories;
using CityInfoAPI.Dtos.Models;
using CityInfoAPI.Service;
using Microsoft.Extensions.Logging;
using Moq;

namespace CityInfoAPI.Test.Tests
{
    public class PointsOfInterestServiceTests
    {
        // The service is handling the automapping //

        private PointsOfInterestService _pointsOfInterestService;
        private readonly Mock<IPointsOfInterestRepository> _repo = new Mock<IPointsOfInterestRepository>();
        private readonly IMapper _mapper;
        private readonly Mock<ILogger<PointsOfInterestService>> _logger = new Mock<ILogger<PointsOfInterestService>>();
        private readonly Guid _cityGuid1 = new Guid("38276231-1918-452d-a3e9-6f50873a95d2");
        private readonly Guid _cityGuid2 = new Guid("09fdd26e-5141-416c-a590-7eaf193b9565");
        private readonly Guid _pointGuid1 = new Guid("e5a5f605-627d-4aec-9f5c-e9939ea0a6cf");
        //private readonly Guid _invalidPointGuid = Guid.Parse("");

        public PointsOfInterestServiceTests()
        {
            _mapper = SetUpAutoMapper.SetUp();
            _pointsOfInterestService = new PointsOfInterestService(_repo.Object, _mapper, _logger.Object);
        }

        // filter, searches //

        [Fact]
        public async Task GetPointsOfInterestAsync_RequestingPOIs_ReturnsCollectionOfPointsOfInterestDtos()
        {
            // arrange - build moq'd repo response
            var pointsOfInterest = new List<PointOfInterest>()
            {
                new PointOfInterest
                {
                    PointGuid =  new Guid("e5a5f605-627d-4aec-9f5c-e9939ea0a6cf"),
                    CityGuid = _cityGuid1,
                    Name = "Lake Michigan**",
                    Description = "Walk along the lake",
                    CreatedOn = new DateTime(2019, 1, 1)
                },
                new PointOfInterest
                {
                    PointGuid =  new Guid("8fb872a7-2559-44b0-b89a-aeea403f58c2"),
                    CityGuid = _cityGuid2,
                    Name = "Lake Docks**",
                    Description = "Rent a boat",
                    CreatedOn = new DateTime(2019, 1, 1)
                }
            };

            // setting up the method of the repo we want to mock.
            // Mock the REPO method - prevents making a true db call.
            // Instructs the moq to return what we just created
            _repo.Setup(x => x.GetPointsOfInterestAsync(string.Empty, string.Empty)).ReturnsAsync(pointsOfInterest);

            // act
            var pointDtos = await _pointsOfInterestService.GetPointsOfInterestAsync(string.Empty, string.Empty);

            // assert
            Assert.True(pointDtos.Count() > 0, "GetPointsOfInterestAsync did not return any Points of Interest.");
        }

        [Fact]
        public async Task GetPointsOfInterestForCityAsync_RequestingPOIs_ReturnsCollectionOfPointsOfInterestDtos()
        {
            // arrange - build moq'd repo response
            var pointsOfInterest = new List<PointOfInterest>()
            {
                new PointOfInterest
                {
                    PointGuid =  new Guid("e5a5f605-627d-4aec-9f5c-e9939ea0a6cf"),
                    CityGuid = _cityGuid1,
                    Name = "Lake Michigan",
                    Description = "Walk along the lake",
                    CreatedOn = new DateTime(2019, 1, 1)
                },
                new PointOfInterest
                {
                    PointGuid =  new Guid("8fb872a7-2559-44b0-b89a-aeea403f58c2"),
                    CityGuid = _cityGuid1,
                    Name = "Lake Docks",
                    Description = "Rent a boat",
                    CreatedOn = new DateTime(2019, 1, 1)
                }
            };

            // setting up the method of the repo we want to mock.
            // Mock the REPO method - prevents making a true db call.
            // Instructs the moq to return what we just created
            _repo.Setup(x => x.GetPointsOfInterestForCityAsync(_cityGuid1)).ReturnsAsync(pointsOfInterest);

            // act
            var pointDtos = await _pointsOfInterestService.GetPointsOfInterestForCityAsync(_cityGuid1);

            // assert
            Assert.True(pointDtos.Count() > 0, "GetPointsOfInterestAsync did not return any Points of Interest.");
        }

        [Fact]
        public async Task GetsPointOfInterestAsync_RequestPOI_ReturnsPointOfInterestDtoType()
        {
            // arrange - build moq'd repo response
            var pointOfInterest = new PointOfInterest()
            {
                PointGuid = _pointGuid1,
                CityGuid = _cityGuid1,
                Name = "Lake Michigan",
                Description = "Walk along the lake",
                CreatedOn = new DateTime(2019, 1, 1)
            };

            // setting up the method of the repo we want to mock.
            // Mock the REPO method - prevents making a true db call.
            // Instructs the moq to return what we just created
            _repo.Setup(x => x.GetPointOfInterestAsync(_pointGuid1)).ReturnsAsync(pointOfInterest);

            // act
            var pointDto = await _pointsOfInterestService.GetPointOfInterestAsync(_pointGuid1);

            // assert
            Assert.IsType<PointOfInterestDto>(pointDto);
        }

        [Fact]
        public async Task PointOfInterestExistsAsync_CheckIfPointExists_ReturnsTrueWithValidGuid()
        {
            // arrange - build moq'd repo response
            // setting up the method of the repo we want to mock.
            // Mock the REPO method - prevents making a true db call.
            // Instructs the moq to return what we just created
            _repo.Setup(x => x.PointOfInterestExistsAsync(_pointGuid1)).ReturnsAsync(true);

            // act
            var results = await _pointsOfInterestService.PointOfInterestExistsAsync(_pointGuid1);

            // assert
            Assert.True(results);
        }

        //[Fact]
        //public async Task PointOfInterestExistsAsync_CheckIfPointExists_ReturnsFalseTrueWithInvalidGuid()
        //{
        //    // arrange - build moq'd repo response
        //    // setting up the method of the repo we want to mock.
        //    // Mock the REPO method - prevents making a true db call.
        //    // Instructs the moq to return what we just created
        //    _repo.Setup(x => x.PointOfInterestExistsAsync(_pointGuid1)).ReturnsAsync(false);

        //    // act
        //    var results = await _pointsOfInterestService.PointOfInterestExistsAsync(_invalidPointGuid);

        //    // assert
        //    Assert.False(results);
        //}
    }
}
