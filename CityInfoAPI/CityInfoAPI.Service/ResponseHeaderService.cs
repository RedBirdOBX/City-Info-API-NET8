using CityInfoAPI.Data.Repositories;
using CityInfoAPI.Dtos;
using Microsoft.Extensions.Logging;

namespace CityInfoAPI.Service
{
    public class ResponseHeaderService : IResponseHeaderService
    {
        private readonly ICityInfoRepository _repo;
        private readonly ILogger<ResponseHeaderService> _logger;

        public ResponseHeaderService(ICityInfoRepository repo, ILogger<ResponseHeaderService> logger)
        {
            _repo = repo;
            _logger = logger;
        }

        public async Task<PaginationMetaDataDto> BuildCitiesHeaderMetaData(int pageNumber, int pageSize)
        {
            try
            {
                var totalCities = await _repo.GetCitiesCountAsync();
                var metaData = MetaDataUtil.BuildCitiesMetaData(totalCities, pageNumber, pageSize);
                return metaData;
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred while building cities header meta data. {ex}");
                throw;
            }
        }
    }
}
