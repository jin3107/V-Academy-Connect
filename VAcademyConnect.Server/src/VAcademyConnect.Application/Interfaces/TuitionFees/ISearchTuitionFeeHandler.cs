using MayNghien.Infrastructures.Models.Requests;
using MayNghien.Infrastructures.Models.Responses;
using VAcademyConnect.DTOs.Responses;

namespace VAcademyConnect.Application.Interfaces.TuitionFees
{
    public interface ISearchTuitionFeeHandler
    {
        Task<AppResponse<SearchResponse<TuitionFeeResponse>>> HandleAsync(SearchRequest request);
    }
}
