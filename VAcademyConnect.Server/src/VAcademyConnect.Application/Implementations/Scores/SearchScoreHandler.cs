using LinqKit;
using MayNghien.Infrastructures.Models.Requests;
using MayNghien.Infrastructures.Models.Responses;
using Microsoft.EntityFrameworkCore;
using VAcademyConnect.Application.Interfaces;
using VAcademyConnect.Application.Interfaces.Repositories;
using VAcademyConnect.Application.Interfaces.Scores;
using VAcademyConnect.Application.Mapping;
using VAcademyConnect.DTOs.Responses;
using VAcademyConnect.Domain.Entities;
using static MayNghien.Infrastructures.Helpers.SearchHelper;

namespace VAcademyConnect.Application.Implementations.Scores
{
    public class SearchScoreHandler : ISearchScoreHandler
    {
        private readonly IScoreRepository _scoreRepository;
        private readonly ICurrentUserService _currentUserService;

        public SearchScoreHandler(IScoreRepository scoreRepository, ICurrentUserService currentUserService)
        {
            _scoreRepository = scoreRepository;
            _currentUserService = currentUserService;
        }

        public async Task<AppResponse<SearchResponse<ScoreResponse>>> HandleAsync(SearchRequest request)
        {
            var result = new AppResponse<SearchResponse<ScoreResponse>>();
            try
            {
                var user = await _currentUserService.GetCurrentUserAsync();
                if (user == null)
                    return result.BuildError("Unauthorize");

                var query = BuildFilterExpression(request.Filters!);
                var numOfRecords = await _scoreRepository.CountRecordsAsync(query);
                var scores = _scoreRepository.FindByPredicate(query)
                    .Include(x => x.Class).Include(x => x.Student).AsQueryable();

                if (request.SortBy != null)
                    scores = _scoreRepository.AddSort(scores, request.SortBy);
                else
                    scores = scores.OrderByDescending(x => x.CreatedOn);

                int pageIndex = request.PageIndex ?? 1;
                int pageSize = request.PageSize ?? 10;
                int startIndex = (pageIndex - 1) * pageSize;
                var scoreList = await scores.Skip(startIndex).Take(pageSize).ToListAsync();
                var dtoList = scoreList.Select(ScoreMapper.ToResponse).ToList();

                var searchResponse = new SearchResponse<ScoreResponse>
                {
                    TotalPages = CalculateNumOfPages(numOfRecords, pageSize),
                    TotalRows = numOfRecords,
                    CurrentPage = pageIndex,
                    Data = dtoList,
                    RowsPerPage = pageSize,
                };
                result.Data = searchResponse;
                result.IsSuccess = true;
            }
            catch (Exception ex)
            {
                result.BuildError(ex.Message + " " + ex.StackTrace);
            }
            return result;
        }

        private ExpressionStarter<Score> BuildFilterExpression(List<Filter> filters)
        {
            try
            {
                var predicate = PredicateBuilder.New<Score>(true);
                if (filters != null)
                {
                    foreach (var filter in filters)
                    {
                        switch (filter.FieldName)
                        {
                            case "ClassId":
                                if (Guid.TryParse(filter.Value, out var classId))
                                    predicate = predicate.And(x => x.ClassId == classId);
                                break;

                            case "StudentId":
                                if (!string.IsNullOrEmpty(filter.Value))
                                    predicate = predicate.And(x => x.StudentId == filter.Value);
                                break;

                            case "TestName":
                                if (!string.IsNullOrEmpty(filter.Value))
                                    predicate = predicate.And(x => x.TestName.Contains(filter.Value));
                                break;

                            default: break;
                        }
                    }
                }

                predicate = predicate.And(x => x.IsDeleted == false);
                return predicate;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message + " " + ex.StackTrace);
            }
        }
    }
}
