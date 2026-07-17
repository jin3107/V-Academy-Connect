using MayNghien.Infrastructures.Models.Responses;
using VAcademyConnect.DTOs.Responses;

namespace VAcademyConnect.Application.Interfaces.TuitionFees
{
    public interface IGetTuitionFeeByIdHandler
    {
        Task<AppResponse<TuitionFeeResponse>> HandleAsync(Guid id);
    }
}
