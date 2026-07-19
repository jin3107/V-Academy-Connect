using MayNghien.Infrastructures.Models.Responses;
using VAcademyConnect.DTOs.Responses;

namespace VAcademyConnect.Application.Interfaces.BackgroundJobs
{
    public interface IGetBackgroundJobByIdHandler
    {
        Task<AppResponse<BackgroundJobResponse>> HandleAsync(Guid id);
    }
}
