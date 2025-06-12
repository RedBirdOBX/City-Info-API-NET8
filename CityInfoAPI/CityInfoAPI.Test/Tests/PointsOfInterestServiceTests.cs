using AutoMapper;
using CityInfoAPI.Data;
using CityInfoAPI.Data.Repositories;
using CityInfoAPI.Dtos;
using CityInfoAPI.Service;
using Microsoft.Extensions.Logging;
using Moq;

namespace CityInfoAPI.Test.Tests;

public class PointsOfInterestServiceTests
{
    // The service is handling the automapping //

    private PointsOfInterestService _pointsOfInterestService;
    private readonly Mock<IPointsOfInterestRepository> _repo = new Mock<IPointsOfInterestRepository>();
    private readonly IMapper _mapper;
    private readonly Mock<ILogger<PointsOfInterestService>> _logger = new Mock<ILogger<PointsOfInterestService>>();
    private readonly Guid _cityGuid1 = new Guid("38276231-1918-452d-a3e9-6f50873a95d2");
    private readonly Guid _pointGuid1 = new Guid("e5a5f605-627d-4aec-9f5c-e9939ea0a6cf");

    public PointsOfInterestServiceTests()
    {
        _mapper = SetUpAutoMapper.SetUp();
        _pointsOfInterestService = new PointsOfInterestService(_repo.Object, _mapper, _logger.Object);
    }


    [Fact]
    [Trait("Category","PointsOfInterest Service Tests")]
    public async Task GetPointsOfInterestAsync_RequestingPOIs_ReturnsCollectionOfPointsOfInterestDtos()
    {
        // arrange - build moq'd repo response
        var pointsOfInterest = new CityInfoTestEntityData().PointsOfInterest;
        _repo.Setup(x => x.GetPointsOfInterestAsync(string.Empty, string.Empty)).ReturnsAsync(pointsOfInterest);

        // act
        var pointDtos = await _pointsOfInterestService.GetPointsOfInterestAsync(string.Empty, string.Empty);

        // assert
        Assert.True(pointDtos.Count() > 0, "GetPointsOfInterestAsync did not return any Points of Interest.");
    }

    [Fact]
    [Trait("Category","PointsOfInterest Service Tests")]

    public async Task GetPointsOfInterestForCityAsync_RequestingPOIs_ReturnsCollectionOfPointsOfInterestDtos()
    {
        // arrange - build moq'd repo response
        var pointsOfInterest = new CityInfoTestEntityData().PointsOfInterest.Where(c=> c.CityGuid == _cityGuid1);
        _repo.Setup(x => x.GetPointsOfInterestForCityAsync(_cityGuid1)).ReturnsAsync(pointsOfInterest);

        // act
        var pointDtos = await _pointsOfInterestService.GetPointsOfInterestForCityAsync(_cityGuid1);

        // assert
        Assert.True(pointDtos.Count() > 0, "GetPointsOfInterestAsync did not return any Points of Interest.");
    }

    [Fact]
    [Trait("Category","PointsOfInterest Service Tests")]
    public async Task GetsPointOfInterestAsync_RequestPOI_ReturnsPointOfInterestDtoType()
    {
        // arrange - build moq'd repo response
        var pointOfInterest = new CityInfoTestEntityData().PointsOfInterest.Where(p=> p.PointGuid == _pointGuid1).FirstOrDefault();
        _repo.Setup(x => x.GetPointOfInterestAsync(_pointGuid1)).ReturnsAsync(pointOfInterest);

        // act
        var pointDto = await _pointsOfInterestService.GetPointOfInterestAsync(_pointGuid1);

        // assert
        Assert.IsType<PointOfInterestDto>(pointDto);
    }

    [Fact]
    [Trait("Category","PointsOfInterest Service Tests")]
    public async Task PointOfInterestExistsAsync_CheckIfPointExists_ReturnsTrueWithValidGuid()
    {
        // arrange - build moq'd repo response
        _repo.Setup(x => x.PointOfInterestExistsAsync(_pointGuid1)).ReturnsAsync(true);

        // act
        var results = await _pointsOfInterestService.PointOfInterestExistsAsync(_pointGuid1);

        // assert
        Assert.True(results);
    }
}
