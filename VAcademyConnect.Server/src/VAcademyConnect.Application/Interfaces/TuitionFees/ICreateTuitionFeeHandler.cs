using MayNghien.Infrastructures.Models.Responses;
using VAcademyConnect.DTOs.Requests;
using VAcademyConnect.DTOs.Responses;

namespace VAcademyConnect.Application.Interfaces.TuitionFees
{
    public interface ICreateTuitionFeeHandler
    {
        Task<AppResponse<TuitionFeeResponse>> HandleAsync(TuitionFeeRequest request);
    }
}
