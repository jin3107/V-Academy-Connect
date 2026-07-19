using MayNghien.Infrastructures.Models.Responses;
using Microsoft.EntityFrameworkCore;
using VAcademyConnect.Application.Interfaces;
using VAcademyConnect.Application.Interfaces.BackgroundJobs;
using VAcademyConnect.Application.Interfaces.Repositories;
using VAcademyConnect.Application.Mapping;
using VAcademyConnect.DTOs.Responses;

namespace VAcademyConnect.Application.Implementations.BackgroundJobs
{
    public class GetBackgroundJobByIdHandler : IGetBackgroundJobByIdHandler
    {
        private readonly IBackgroundJobRepository _backgroundJobRepository;
        private readonly ICurrentUserService _currentUserService;

        public GetBackgroundJobByIdHandler(IBackgroundJobRepository backgroundJobRepository, ICurrentUserService currentUserService)
        {
            _backgroundJobRepository = backgroundJobRepository;
            _currentUserService = currentUserService;
        }

        public async Task<AppResponse<BackgroundJobResponse>> HandleAsync(Guid id)
        {
            var result = new AppResponse<BackgroundJobResponse>();
            try
            {
                var user = await _currentUserService.GetCurrentUserAsync();
                if (user == null)
                    return result.BuildError("Unauthorize.");

                var backgroundJob = await _backgroundJobRepository.FindByAsync(x => x.Id == id)
                    .Include(x => x.Tenant).FirstOrDefaultAsync();
                if (backgroundJob == null || backgroundJob.IsDeleted)
                    return result.BuildError("Background job record was not found or has been deleted.");

                var response = BackgroundJobMapper.ToResponse(backgroundJob);
                result.BuildResult(response, "Background job record has been found.");
            }
            catch (Exception ex)
            {
                result.BuildError(ex.Message + " " + ex.StackTrace);
            }
            return result;
        }
    }
}
