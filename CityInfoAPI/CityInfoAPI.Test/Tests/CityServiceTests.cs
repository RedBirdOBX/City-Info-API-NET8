using AutoMapper;
using CityInfoAPI.Data;
using CityInfoAPI.Data.Repositories;
using CityInfoAPI.Dtos;
using CityInfoAPI.Service;
using CityInfoAPI.Dtos.RequestModels;
using Microsoft.Extensions.Logging;
using Moq;

namespace CityInfoAPI.Test.Tests;

public class CityServiceTests
{

    private CityService _cityService;
    private readonly Mock<ICitiesRepository> _repo = new Mock<ICitiesRepository>();
    private readonly IMapper _mapper;
    private readonly Mock<ILogger<CityService>> _logger = new Mock<ILogger<CityService>>();
    private readonly Guid _cityGuid = Guid.Parse("38276231-1918-452d-a3e9-6f50873a95d2");
    private readonly Guid _invalidCityGuid = Guid.Parse("e5a5f605-627d-4aec-9f5c-e9939ea0a6cf");

    public CityServiceTests()
    {
        _mapper = SetUpAutoMapper.SetUp();
        _cityService = new CityService(_repo.Object, _mapper, _logger.Object);
    }

    [Fact]
    [Trait("Category", "City Service Tests")]
    public async Task GetCitiesAsync_RequestingCities_ReturnsListOfCityDtosWithNoPointsOfInterest()
    {
        // arrange. build moq'd repo response
        var requestParams = new CityRequestParameters();
        var cities = new CityInfoTestEntityData().Cities;
        _repo.Setup(x => x.GetCitiesAsync(requestParams)).ReturnsAsync(cities);

        // act
        var cityDtos = await _cityService.GetCitiesAsync(requestParams);

        // assert
        Assert.True(cityDtos.Count() > 0, "GetCitiesAsync did not return any Cities.");
        Assert.All(cityDtos, c => Assert.True(!c.PointsOfInterest.Any(), "CityDto contains PointsOfInterests when it should not."));;
    }

    [Fact]
    [Trait("Category", "City Service Tests")]
    public async Task GetCitiesAsync_RequestingCities_ReturnsListOfCityDtosTypes()
    {
        // arrange. build moq'd response
        var requestParams = new CityRequestParameters();
        var cities = new CityInfoTestEntityData().Cities;
        _repo.Setup(x => x.GetCitiesAsync(requestParams)).ReturnsAsync(cities);

        // act
        var cityDtos = await _cityService.GetCitiesAsync(requestParams);

        // assert
        Assert.All(cityDtos, c => Assert.IsType<CityDto>(c));
    }

    [Fact]
    [Trait("Category", "City Service Tests")]
    public async Task GetAllCities_RequestingCities_ReturnsListOfCityWithoutPointOfInterestDtos()
    {
        // arrange. build moq'd response
        var requestParams = new CityRequestParameters();
        var cities = new CityInfoTestEntityData().Cities;
        _repo.Setup(x => x.GetCitiesAsync(requestParams)).ReturnsAsync(cities);

        // act
        var cityDtos = await _cityService.GetCitiesAsync(requestParams);

        // assert
        Assert.True(cityDtos.Count() > 0, "GetCitiesAsync did not return any Cities.");
    }

    [Fact]
    [Trait("Category","City Service Tests")]
    public async Task CityExists_CityIfValidCityExists_ReturnsTrue()
    {
        // arrange. build moq'd response
        _repo.Setup(x => x.CityExistsAsync(_cityGuid)).ReturnsAsync(true);

        // act
        var response = await _cityService.CityExistsAsync(_cityGuid);

        // assert
        Assert.True(response);
    }

    [Fact]
    [Trait("Category","City Service Tests")]
    public async Task CityExists_CityIfInvalidCityExists_ReturnsFalse()
    {
        // arrange. build moq'd response
        _repo.Setup(x => x.CityExistsAsync(_invalidCityGuid)).ReturnsAsync(false);

        // act
        var response = await _cityService.CityExistsAsync(_cityGuid);

        // assert
        Assert.False(response);
    }

    [Fact]
    [Trait("Category", "City Service Tests")]
    public async Task GetCitiesAsync_PagingWorksProperty_ProperNumberOfResultsAreSkipped()
    {
        // arrange
        var requestParams = new CityRequestParameters
        {
            PageNumber = 2,
            PageSize = 5,
            Name = string.Empty,
            Search = string.Empty,
            IncludePointsOfInterest = false
        };

        // should skip first 5
        var cities = new CityInfoTestEntityData().Cities.Skip((requestParams.PageNumber - 1) * requestParams.PageSize).Take(requestParams.PageSize);
        _repo.Setup(x => x.GetCitiesAsync(requestParams)).ReturnsAsync(cities);

        // act
        var response = await _cityService.GetCitiesAsync(requestParams);

        // assert - should return 5
        Assert.True(response.Count() == 5);
    }

    [Fact]
    [Trait("Category", "City Service Tests")]
    public async Task GetCitiesAsync_NameSearch_ProperlyFindsCitiesMatchingSearchCriteria()
    {
        // arrange
        var requestParams = new CityRequestParameters
        {
            PageNumber = 1,
            PageSize = 10,
            Name = string.Empty,
            Search = "the",
            IncludePointsOfInterest = false
        };

        var cities = new CityInfoTestEntityData().Cities.Where(c => c.Name.Contains(requestParams.Search) || c.Description.Contains(requestParams.Search)).Take(requestParams.PageSize);
        _repo.Setup(x => x.GetCitiesAsync(requestParams)).ReturnsAsync(cities);

        // act
        var response = await _cityService.GetCitiesAsync(requestParams);

        // assert - should return 4
        Assert.True(response.Count() == 4);
    }

    [Fact]
    [Trait("Category", "City Service Tests")]
    public async Task GetCitiesAsync_NameFilter_ProperlyFindsCitiesMatchingName()
    {
        // arrange
        var requestParams = new CityRequestParameters
        {
            PageNumber = 1,
            PageSize = 10,
            Name = "Richmond (in memory)",
            Search = string.Empty,
            IncludePointsOfInterest = false
        };

        var cities = new CityInfoTestEntityData().Cities.Where(c => c.Name.ToLower().Equals(requestParams.Name.ToLower()));
        _repo.Setup(x => x.GetCitiesAsync(requestParams)).ReturnsAsync(cities);

        // act
        var response = await _cityService.GetCitiesAsync(requestParams);

        // assert - should return
        Assert.True(response.Count() == 1);
    }
}