using MayNghien.Infrastructures.Models.Responses;
using VAcademyConnect.Application.Interfaces;
using VAcademyConnect.Application.Interfaces.Classes;
using VAcademyConnect.Application.Interfaces.Repositories;

namespace VAcademyConnect.Application.Implementations.Classes
{
    public class DeleteClassHandler : IDeleteClassHandler
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly IClassRepository _classRepository;

        public DeleteClassHandler(ICurrentUserService currentUserService, IClassRepository classRepository)
        {
            _currentUserService = currentUserService;
            _classRepository = classRepository;
        }

        public async Task<AppResponse<string>> HandleAsync(Guid id)
        {
            var result = new AppResponse<string>();
            try
            {
                var user = await _currentUserService.GetCurrentUserAsync();
                if (user == null)
                    return result.BuildError("Unauthorize");

                var classes = await _classRepository.GetAsync(id);
                if (classes == null || classes.IsDeleted == true)
                    return result.BuildError("Class was not found or has been deleted.");

                classes.IsDeleted = true;
                await _classRepository.EditAsync(classes);
                result.BuildResult("Class has been deleted successfully.");
            }
            catch (Exception ex)
            {
                result.BuildError(ex.Message + " " + ex.StackTrace);
            }
            return result;
        }
    }
}
