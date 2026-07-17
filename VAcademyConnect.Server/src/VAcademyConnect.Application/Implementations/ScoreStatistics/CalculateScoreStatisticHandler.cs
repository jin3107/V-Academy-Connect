using MayNghien.Infrastructures.Models.Responses;
using Microsoft.EntityFrameworkCore;
using VAcademyConnect.Application.Interfaces;
using VAcademyConnect.Application.Interfaces.Repositories;
using VAcademyConnect.Application.Interfaces.ScoreStatistics;
using VAcademyConnect.Application.Mapping;
using VAcademyConnect.DTOs.Requests;
using VAcademyConnect.DTOs.Responses;
using VAcademyConnect.Domain.Entities;

namespace VAcademyConnect.Application.Implementations.ScoreStatistics
{
    public class CalculateScoreStatisticHandler : ICalculateScoreStatisticHandler
    {
        private readonly IScoreRepository _scoreRepository;
        private readonly IScoreStatisticRepository _scoreStatisticRepository;
        private readonly ICurrentUserService _currentUserService;

        public CalculateScoreStatisticHandler(IScoreRepository scoreRepository,
            IScoreStatisticRepository scoreStatisticRepository, ICurrentUserService currentUserService)
        {
            _scoreRepository = scoreRepository;
            _scoreStatisticRepository = scoreStatisticRepository;
            _currentUserService = currentUserService;
        }

        public async Task<AppResponse<List<ScoreStatisticResponse>>> HandleAsync(ScoreStatisticCalculateRequest request)
        {
            var result = new AppResponse<List<ScoreStatisticResponse>>();
            try
            {
                var user = await _currentUserService.GetCurrentUserAsync();
                if (user == null)
                    return result.BuildError("Unauthorize.");

                var monthStart = DateTime.ParseExact(request.Month + "-01", "yyyy-MM-dd", null);
                var monthEnd = monthStart.AddMonths(1);

                var scoreQuery = _scoreRepository.FindByAsync(s => !s.IsDeleted && s.StudentId != null &&
                    s.CreatedOn >= monthStart && s.CreatedOn < monthEnd);
                if (request.ClassId.HasValue)
                    scoreQuery = scoreQuery.Where(s => s.ClassId == request.ClassId);

                var averages = await scoreQuery
                    .GroupBy(s => s.StudentId)
                    .Select(g => new { StudentId = g.Key!, Average = g.Average(s => s.Mark) })
                    .ToListAsync();

                var responses = new List<ScoreStatisticResponse>();
                foreach (var average in averages)
                {
                    var existing = await _scoreStatisticRepository
                        .FindByAsync(x => x.StudentId == average.StudentId && x.Month == request.Month && !x.IsDeleted)
                        .Include(x => x.Student).FirstOrDefaultAsync();

                    if (existing != null)
                    {
                        existing.AvarageScore = Math.Round(average.Average, 2);
                        existing.ModifiedBy = user.Email;
                        existing.ModifiedOn = DateTime.UtcNow;
                        await _scoreStatisticRepository.EditAsync(existing);
                        responses.Add(ScoreStatisticMapper.ToResponse(existing));
                    }
                    else
                    {
                        var newStatistic = new ScoreStatistic
                        {
                            Id = Guid.NewGuid(),
                            StudentId = average.StudentId,
                            Month = request.Month,
                            AvarageScore = Math.Round(average.Average, 2),
                            CreatedBy = user.Email,
                            CreatedOn = DateTime.UtcNow,
                        };
                        await _scoreStatisticRepository.AddAsync(newStatistic);

                        var createdEntity = await _scoreStatisticRepository.FindByAsync(x => x.Id == newStatistic.Id)
                            .Include(x => x.Student).FirstOrDefaultAsync();
                        responses.Add(ScoreStatisticMapper.ToResponse(createdEntity ?? newStatistic));
                    }
                }

                result.BuildResult(responses, $"Calculated average scores for {responses.Count} student(s) in {request.Month}.");
            }
            catch (Exception ex)
            {
                result.BuildError(ex.Message + " " + ex.StackTrace);
            }
            return result;
        }
    }
}
