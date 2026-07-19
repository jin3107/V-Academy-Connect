using LinqKit;
using MayNghien.Infrastructures.Models.Requests;
using MayNghien.Infrastructures.Models.Responses;
using Microsoft.EntityFrameworkCore;
using VAcademyConnect.Application.Interfaces;
using VAcademyConnect.Application.Interfaces.PaymentChecks;
using VAcademyConnect.Application.Interfaces.Repositories;
using VAcademyConnect.Application.Mapping;
using VAcademyConnect.DTOs.Responses;
using VAcademyConnect.Domain.Entities;
using static MayNghien.Infrastructures.Helpers.SearchHelper;

namespace VAcademyConnect.Application.Implementations.PaymentChecks
{
    public class SearchPaymentCheckHandler : ISearchPaymentCheckHandler
    {
        private readonly IPaymentCheckRepository _paymentCheckRepository;
        private readonly ICurrentUserService _currentUserService;

        public SearchPaymentCheckHandler(IPaymentCheckRepository paymentCheckRepository, ICurrentUserService currentUserService)
        {
            _paymentCheckRepository = paymentCheckRepository;
            _currentUserService = currentUserService;
        }

        public async Task<AppResponse<SearchResponse<PaymentCheckResponse>>> HandleAsync(SearchRequest request)
        {
            var result = new AppResponse<SearchResponse<PaymentCheckResponse>>();
            try
            {
                var user = await _currentUserService.GetCurrentUserAsync();
                if (user == null)
                    return result.BuildError("Unauthorize");

                var query = BuildFilterExpression(request.Filters!);
                var numOfRecords = await _paymentCheckRepository.CountRecordsAsync(query);
                var paymentChecks = _paymentCheckRepository.FindByPredicate(query)
                    .Include(x => x.CheckedByUser).AsQueryable();

                if (request.SortBy != null)
                    paymentChecks = _paymentCheckRepository.AddSort(paymentChecks, request.SortBy);
                else
                    paymentChecks = paymentChecks.OrderByDescending(x => x.CheckTime);

                int pageIndex = request.PageIndex ?? 1;
                int pageSize = request.PageSize ?? 10;
                int startIndex = (pageIndex - 1) * pageSize;
                var paymentCheckList = await paymentChecks.Skip(startIndex).Take(pageSize).ToListAsync();
                var dtoList = paymentCheckList.Select(PaymentCheckMapper.ToResponse).ToList();

                var searchResponse = new SearchResponse<PaymentCheckResponse>
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

        private ExpressionStarter<PaymentCheck> BuildFilterExpression(List<Filter> filters)
        {
            try
            {
                var predicate = PredicateBuilder.New<PaymentCheck>(true);
                if (filters != null)
                {
                    foreach (var filter in filters)
                    {
                        switch (filter.FieldName)
                        {
                            case "PaymentId":
                                if (!string.IsNullOrEmpty(filter.Value) && Guid.TryParse(filter.Value, out var paymentId))
                                    predicate = predicate.And(x => x.PaymentId == paymentId);
                                break;

                            case "CheckedBy":
                                if (!string.IsNullOrEmpty(filter.Value))
                                    predicate = predicate.And(x => x.CheckedBy == filter.Value);
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
