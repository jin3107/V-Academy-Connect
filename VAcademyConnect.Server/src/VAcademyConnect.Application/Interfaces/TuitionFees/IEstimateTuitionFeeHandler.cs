using MayNghien.Infrastructures.Models.Responses;
using VAcademyConnect.DTOs.Requests;
using VAcademyConnect.DTOs.Responses;

namespace VAcademyConnect.Application.Interfaces.TuitionFees
{
    public interface IEstimateTuitionFeeHandler
    {
        Task<AppResponse<TuitionFeeResponse>> HandleAsync(TuitionFeeEstimateRequest request);
    }
}
