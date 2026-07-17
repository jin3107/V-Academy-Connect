using LinqKit;
using MayNghien.Infrastructures.Models.Requests;
using MayNghien.Infrastructures.Models.Responses;
using Microsoft.EntityFrameworkCore;
using VAcademyConnect.Application.Interfaces;
using VAcademyConnect.Application.Interfaces.Repositories;
using VAcademyConnect.Application.Interfaces.ScoreStatistics;
using VAcademyConnect.Application.Mapping;
using VAcademyConnect.DTOs.Responses;
using VAcademyConnect.Domain.Entities;
using static MayNghien.Infrastructures.Helpers.SearchHelper;

namespace VAcademyConnect.Application.Implementations.ScoreStatistics
{
    public class SearchScoreStatisticHandler : ISearchScoreStatisticHandler
    {
        private readonly IScoreStatisticRepository _scoreStatisticRepository;
        private readonly ICurrentUserService _currentUserService;

        public SearchScoreStatisticHandler(IScoreStatisticRepository scoreStatisticRepository, ICurrentUserService currentUserService)
        {
            _scoreStatisticRepository = scoreStatisticRepository;
            _currentUserService = currentUserService;
        }

        public async Task<AppResponse<SearchResponse<ScoreStatisticResponse>>> HandleAsync(SearchRequest request)
        {
            var result = new AppResponse<SearchResponse<ScoreStatisticResponse>>();
            try
            {
                var user = await _currentUserService.GetCurrentUserAsync();
                if (user == null)
                    return result.BuildError("Unauthorize");

                var query = BuildFilterExpression(request.Filters!);
                var numOfRecords = await _scoreStatisticRepository.CountRecordsAsync(query);
                var scoreStatistics = _scoreStatisticRepository.FindByPredicate(query)
                    .Include(x => x.Student).AsQueryable();

                if (request.SortBy != null)
                    scoreStatistics = _scoreStatisticRepository.AddSort(scoreStatistics, request.SortBy);
                else
                    scoreStatistics = scoreStatistics.OrderByDescending(x => x.Month);

                int pageIndex = request.PageIndex ?? 1;
                int pageSize = request.PageSize ?? 10;
                int startIndex = (pageIndex - 1) * pageSize;
                var scoreStatisticList = await scoreStatistics.Skip(startIndex).Take(pageSize).ToListAsync();
                var dtoList = scoreStatisticList.Select(ScoreStatisticMapper.ToResponse).ToList();

                var searchResponse = new SearchResponse<ScoreStatisticResponse>
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

        private ExpressionStarter<ScoreStatistic> BuildFilterExpression(List<Filter> filters)
        {
            try
            {
                var predicate = PredicateBuilder.New<ScoreStatistic>(true);
                if (filters != null)
                {
                    foreach (var filter in filters)
                    {
                        switch (filter.FieldName)
                        {
                            case "StudentId":
                                if (!string.IsNullOrEmpty(filter.Value))
                                    predicate = predicate.And(x => x.StudentId == filter.Value);
                                break;

                            case "Month":
                                if (!string.IsNullOrEmpty(filter.Value))
                                    predicate = predicate.And(x => x.Month == filter.Value);
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
