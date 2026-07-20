using MayNghien.Infrastructures.Models.Responses;

namespace VAcademyConnect.Application.Interfaces.Classes
{
    public interface IDeleteClassHandler
    {
        Task<AppResponse<string>> HandleAsync(Guid id);
    }
}
