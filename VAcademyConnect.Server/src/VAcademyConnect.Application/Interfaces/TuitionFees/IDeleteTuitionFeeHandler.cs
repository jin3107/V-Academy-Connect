using MayNghien.Infrastructures.Models.Responses;

namespace VAcademyConnect.Application.Interfaces.TuitionFees
{
    public interface IDeleteTuitionFeeHandler
    {
        Task<AppResponse<string>> HandleAsync(Guid id);
    }
}
