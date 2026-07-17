using LinqKit;
using MayNghien.Infrastructures.Models.Requests;
using MayNghien.Infrastructures.Models.Responses;
using Microsoft.EntityFrameworkCore;
using VAcademyConnect.Application.Interfaces;
using VAcademyConnect.Application.Interfaces.Repositories;
using VAcademyConnect.Application.Interfaces.Staffs;
using VAcademyConnect.Application.Mapping;
using VAcademyConnect.DTOs.Responses;
using VAcademyConnect.Domain.Entities;
using static MayNghien.Infrastructures.Helpers.SearchHelper;

namespace VAcademyConnect.Application.Implementations.Staffs
{
    public class SearchStaffHandler : ISearchStaffHandler
    {
        private readonly IStaffRepository _staffRepository;
        private readonly ICurrentUserService _currentUserService;

        public SearchStaffHandler(IStaffRepository staffRepository, ICurrentUserService currentUserService)
        {
            _staffRepository = staffRepository;
            _currentUserService = currentUserService;
        }

        public async Task<AppResponse<SearchResponse<StaffResponse>>> HandleAsync(SearchRequest request)
        {
            var result = new AppResponse<SearchResponse<StaffResponse>>();
            try
            {
                var user = await _currentUserService.GetCurrentUserAsync();
                if (user == null)
                    return result.BuildError("Unauthorize");

                var query = BuildFilterExpression(request.Filters!);
                if (user.Role != Commons.Enums.Role.SuperAdmin && user.TenantId.HasValue)
                    query = query.And(x => x.TenantId == user.TenantId);

                var numOfRecords = await _staffRepository.CountRecordsAsync(query);
                var staffs = _staffRepository.FindByPredicate(query).AsQueryable();
                if (request.SortBy != null)
                    staffs = _staffRepository.AddSort(staffs, request.SortBy);
                else
                    staffs = staffs.OrderBy(x => x.Name);

                int pageIndex = request.PageIndex ?? 1;
                int pageSize = request.PageSize ?? 1;
                int startIndex = (pageIndex - 1) * pageSize;
                var staffList = await staffs.Skip(startIndex).Take(pageSize).ToListAsync();
                var dtoList = staffList.Select(StaffMapper.ToResponse).ToList();
                var searchResponse = new SearchResponse<StaffResponse>
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

        private ExpressionStarter<Staff> BuildFilterExpression(List<Filter> filters)
        {
            try
            {
                var predicate = PredicateBuilder.New<Staff>(true);
                if (filters != null)
                {
                    foreach (var filter in filters)
                    {
                        switch (filter.FieldName)
                        {
                            case "Name":
                                if (!string.IsNullOrEmpty(filter.Value))
                                    predicate = predicate.And(x => x.Name.Contains(filter.Value));
                                break;

                            case "Email":
                                if (!string.IsNullOrEmpty(filter.Value))
                                    predicate = predicate.And(x => x.Email.Contains(filter.Value));
                                break;

                            case "PhoneNumber":
                                if (!string.IsNullOrEmpty(filter.Value))
                                    predicate = predicate.And(x => x.PhoneNumber.Contains(filter.Value));
                                break;

                            case "Role":
                                if (!string.IsNullOrEmpty(filter.Value))
                                    predicate = predicate.And(x => x.Role.Contains(filter.Value));
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
