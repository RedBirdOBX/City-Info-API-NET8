using CityInfoAPI.Dtos;

namespace CityInfoAPI.Service
{
    public interface IResponseHeaderService
    {
        Task<PaginationMetaDataDto> BuildCitiesHeaderMetaData(int pageNumber, int pageSize);
    }
}