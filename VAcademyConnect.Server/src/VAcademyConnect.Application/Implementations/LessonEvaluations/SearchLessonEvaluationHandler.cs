using LinqKit;
using MayNghien.Infrastructures.Models.Requests;
using MayNghien.Infrastructures.Models.Responses;
using Microsoft.EntityFrameworkCore;
using VAcademyConnect.Application.Interfaces;
using VAcademyConnect.Application.Interfaces.LessonEvaluations;
using VAcademyConnect.Application.Interfaces.Repositories;
using VAcademyConnect.Application.Mapping;
using VAcademyConnect.DTOs.Responses;
using VAcademyConnect.Domain.Entities;
using static MayNghien.Infrastructures.Helpers.SearchHelper;

namespace VAcademyConnect.Application.Implementations.LessonEvaluations
{
    public class SearchLessonEvaluationHandler : ISearchLessonEvaluationHandler
    {
        private readonly ILessonEvaluationRepository _lessonEvaluationRepository;
        private readonly ICurrentUserService _currentUserService;

        public SearchLessonEvaluationHandler(ILessonEvaluationRepository lessonEvaluationRepository, ICurrentUserService currentUserService)
        {
            _lessonEvaluationRepository = lessonEvaluationRepository;
            _currentUserService = currentUserService;
        }

        public async Task<AppResponse<SearchResponse<LessonEvaluationResponse>>> HandleAsync(SearchRequest request)
        {
            var result = new AppResponse<SearchResponse<LessonEvaluationResponse>>();
            try
            {
                var user = await _currentUserService.GetCurrentUserAsync();
                if (user == null)
                    return result.BuildError("Unauthorize");

                var query = BuildFilterExpression(request.Filters!);
                var numOfRecords = await _lessonEvaluationRepository.CountRecordsAsync(query);
                var lessonEvaluations = _lessonEvaluationRepository.FindByPredicate(query)
                    .Include(x => x.Class).Include(x => x.Student).AsQueryable();

                if (request.SortBy != null)
                    lessonEvaluations = _lessonEvaluationRepository.AddSort(lessonEvaluations, request.SortBy);
                else
                    lessonEvaluations = lessonEvaluations.OrderByDescending(x => x.Date);

                int pageIndex = request.PageIndex ?? 1;
                int pageSize = request.PageSize ?? 10;
                int startIndex = (pageIndex - 1) * pageSize;
                var lessonEvaluationList = await lessonEvaluations.Skip(startIndex).Take(pageSize).ToListAsync();
                var dtoList = lessonEvaluationList.Select(LessonEvaluationMapper.ToResponse).ToList();

                var searchResponse = new SearchResponse<LessonEvaluationResponse>
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

        private ExpressionStarter<LessonEvaluation> BuildFilterExpression(List<Filter> filters)
        {
            try
            {
                var predicate = PredicateBuilder.New<LessonEvaluation>(true);
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

                            case "Date":
                                if (DateTime.TryParse(filter.Value, out var date))
                                    predicate = predicate.And(x => x.Date != null && x.Date.Value.Date == date.Date);
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
