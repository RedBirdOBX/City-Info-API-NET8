using AutoMapper;
using CityInfoAPI.Data.Entities;
using CityInfoAPI.Data.Repositories;
using CityInfoAPI.Dtos.Models;
using CityInfoAPI.Service;
using Microsoft.Extensions.Logging;
using Moq;

namespace CityInfoAPI.Test
{
    public class CityServiceTests
    {

        private CityService _cityService;
        private readonly Mock<ICityRepository> _repo = new Mock<ICityRepository>();
        private readonly IMapper _mapper;
        private readonly Mock<ILogger<CityService>> _logger = new Mock<ILogger<CityService>>();

        public CityServiceTests()
        {
            _mapper = SetUpAutoMapper.SetUp();
            _cityService = new CityService(_repo.Object, _mapper, _logger.Object);
        }

        [Fact]
        public async Task GetCities_RequestingCities_ReturnsListOfCityWithoutPointOfInterestDtos()
        {
            // arrange. build moq'd response
            var cities = new List<City>
            {
                new City()
                {
                    CityGuid = Guid.NewGuid(),
                    Name = "City 1",
                    Description = "Description 1",
                    CreatedOn = DateTime.Today,
                },
                new City()
                {
                    CityGuid = Guid.NewGuid(),
                    Name = "City 2",
                    Description = "Description 2",
                    CreatedOn = DateTime.Today,
                }
            };

            // setting up the method of the repo we want to mock.
            // Mock the REPO method - prevents making a true db call.
            // Instructs the moq to return what we just created
            _repo.Setup(x => x.GetCitiesAsync(string.Empty, string.Empty, 1, 100)).ReturnsAsync(cities);

            // act
            var cityDtos = await _cityService.GetCitiesAsync(string.Empty,string.Empty, 1, 100);

            // assert
            Assert.True(cityDtos.Count() > 0, "GetCitiesAsync did not return any Cities.");
        }

        [Fact]
        public async Task GetCities_RequestingCities_ReturnsListOfTypeCityWithoutPointOfInterestDtos()
        {
            // arrange. build moq'd response
            var cities = new List<City>
            {
                new City()
                {
                    CityGuid = Guid.NewGuid(),
                    Name = "City 1",
                    Description = "Description 1",
                    CreatedOn = DateTime.Today,
                },
                new City()
                {
                    CityGuid = Guid.NewGuid(),
                    Name = "City 2",
                    Description = "Description 2",
                    CreatedOn = DateTime.Today,
                }
            };

            // setting up the method of the repo we want to mock.
            // Mock the REPO method - prevents making a true db call.
            // Instructs the moq to return what we just created
            _repo.Setup(x => x.GetCitiesAsync(string.Empty, string.Empty, 1, 100)).ReturnsAsync(cities);

            // act
            var cityDtos = await _cityService.GetCitiesAsync(string.Empty,string.Empty, 1, 100);

            // assert
            Assert.All(cityDtos, c => Assert.IsType<CityWithoutPointsOfInterestDto>(c));
        }

        [Fact]
        public async Task GetAllCities_RequestingCities_ReturnsListOfCityWithoutPointOfInterestDtos()
        {
            // arrange. build moq'd response
            var cities = new List<City>
            {
                new City()
                {
                    CityGuid = Guid.NewGuid(),
                    Name = "City 1",
                    Description = "Description 1",
                    CreatedOn = DateTime.Today,
                },
                new City()
                {
                    CityGuid = Guid.NewGuid(),
                    Name = "City 2",
                    Description = "Description 2",
                    CreatedOn = DateTime.Today,
                }
            };

            // setting up the method of the repo we want to mock.
            // Mock the REPO method - prevents making a true db call.
            // Instructs the moq to return what we just created
            _repo.Setup(x => x.GetCitiesUnsortedAsync()).ReturnsAsync(cities);

            // act
            var cityDtos = await _cityService.GetAllCitiesAsync();

            // assert
            Assert.True(cityDtos.Count() > 0, "GetCitiesAsync did not return any Cities.");
        }

        [Fact]
        public async Task GetAllCities_RequestingCities_ReturnsListOfTypeCityWithoutPointOfInterestDtos()
        {
            // arrange. build moq'd response
            var cities = new List<City>
            {
                new City()
                {
                    CityGuid = Guid.NewGuid(),
                    Name = "City 1",
                    Description = "Description 1",
                    CreatedOn = DateTime.Today,
                },
                new City()
                {
                    CityGuid = Guid.NewGuid(),
                    Name = "City 2",
                    Description = "Description 2",
                    CreatedOn = DateTime.Today,
                }
            };

            // setting up the method of the repo we want to mock.
            // Mock the REPO method - prevents making a true db call.
            // Instructs the moq to return what we just created
            _repo.Setup(x => x.GetCitiesUnsortedAsync()).ReturnsAsync(cities);

            // act
            var cityDtos = await _cityService.GetAllCitiesAsync();

            // assert
            Assert.All(cityDtos, c => Assert.IsType<CityWithoutPointsOfInterestDto>(c));
        }

        [Fact]
        public async Task CityExists_CityIfValidCityExists_ReturnsTrue()
        {
            // arrange. build moq'd response
            var cityGuid = Guid.Parse("38276231-1918-452d-a3e9-6f50873a95d2");
            var city = new City()
            {
                CityGuid = cityGuid,
                Name = "City 1",
                Description = "Description 1",
                CreatedOn = DateTime.Today,
            };

            // setting up the method of the repo we want to mock.
            // Mock the REPO method - prevents making a true db call.
            // Instructs the moq to return what we just created
            _repo.Setup(x => x.CityExistsAsync(cityGuid)).ReturnsAsync(true);

            // act
            var response = await _cityService.CityExistsAsync(cityGuid);

            // assert
            Assert.True(response);
        }

        [Fact]
        public async Task CityExists_CityIfInvalidCityExists_ReturnsFalse()
        {
            // arrange. build moq'd response
            var invalidCityGuid = Guid.Parse("e5a5f605-627d-4aec-9f5c-e9939ea0a6cf");
            var cityGuid = Guid.Parse("38276231-1918-452d-a3e9-6f50873a95d2");
            var city = new City()
            {
                CityGuid = cityGuid,
                Name = "City 1",
                Description = "Description 1",
                CreatedOn = DateTime.Today,
            };

            // setting up the method of the repo we want to mock.
            // Mock the REPO method - prevents making a true db call.
            // Instructs the moq to return what we just created
            _repo.Setup(x => x.CityExistsAsync(invalidCityGuid)).ReturnsAsync(false);

            // act
            var response = await _cityService.CityExistsAsync(cityGuid);

            // assert
            Assert.False(response);
        }
    }
}