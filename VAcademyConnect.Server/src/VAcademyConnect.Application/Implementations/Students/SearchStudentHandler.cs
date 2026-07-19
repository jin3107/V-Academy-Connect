using MayNghien.Infrastructures.Models.Requests;
using MayNghien.Infrastructures.Models.Responses;
using VAcademyConnect.Application.Interfaces;
using VAcademyConnect.Application.Interfaces.Repositories;
using VAcademyConnect.Application.Interfaces.Students;
using VAcademyConnect.Application.Mapping;
using VAcademyConnect.DTOs.Responses;
using VAcademyConnect.Domain.Entities;
using static MayNghien.Infrastructures.Helpers.SearchHelper;

namespace VAcademyConnect.Application.Implementations.Students
{
    public class SearchStudentHandler : ISearchStudentHandler
    {
        private readonly IUserAccountService _userAccountService;
        private readonly ICurrentUserService _currentUserService;
        private readonly ITenantRepository _tenantRepository;

        public SearchStudentHandler(IUserAccountService userAccountService, ICurrentUserService currentUserService,
            ITenantRepository tenantRepository)
        {
            _userAccountService = userAccountService;
            _currentUserService = currentUserService;
            _tenantRepository = tenantRepository;
        }

        public async Task<AppResponse<SearchResponse<StudentResponse>>> HandleAsync(SearchRequest request)
        {
            var result = new AppResponse<SearchResponse<StudentResponse>>();
            try
            {
                var currentUser = await _currentUserService.GetCurrentUserAsync();
                if (currentUser == null)
                    return result.BuildError("Unauthorize");

                var roles = await _currentUserService.GetRolesAsync(currentUser);
                var isSuperAdmin = roles.Contains("SuperAdmin");

                var students = await _userAccountService.GetUsersInRoleAsync("Student");

                Guid? tenantFilter = isSuperAdmin ? null : currentUser.TenantId;
                string? nameFilter = null;

                foreach (var filter in request.Filters ?? new List<Filter>())
                {
                    switch (filter.FieldName)
                    {
                        case "TenantId":
                            if (isSuperAdmin && !string.IsNullOrEmpty(filter.Value) && Guid.TryParse(filter.Value, out var tid))
                                tenantFilter = tid;
                            break;

                        case "Name":
                            if (!string.IsNullOrEmpty(filter.Value))
                                nameFilter = filter.Value;
                            break;
                    }
                }

                var filtered = students.Where(s =>
                    (!tenantFilter.HasValue || s.TenantId == tenantFilter) &&
                    (string.IsNullOrEmpty(nameFilter) ||
                        (s.FullName?.Contains(nameFilter, StringComparison.OrdinalIgnoreCase) ?? false) ||
                        (s.Email?.Contains(nameFilter, StringComparison.OrdinalIgnoreCase) ?? false)))
                    .OrderBy(s => s.FullName)
                    .ToList();

                int pageIndex = request.PageIndex ?? 1;
                int pageSize = request.PageSize ?? 10;
                var pageItems = filtered.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();

                var tenantIds = pageItems.Where(s => s.TenantId.HasValue).Select(s => s.TenantId!.Value).Distinct().ToList();
                var tenants = new Dictionary<Guid, Tenant>();
                foreach (var tid in tenantIds)
                {
                    var tenant = await _tenantRepository.GetAsync(tid);
                    if (tenant != null) tenants[tid] = tenant;
                }
                foreach (var s in pageItems)
                {
                    if (s.TenantId.HasValue && tenants.TryGetValue(s.TenantId.Value, out var tenant))
                        s.Tenant = tenant;
                }

                var searchResponse = new SearchResponse<StudentResponse>
                {
                    TotalPages = CalculateNumOfPages(filtered.Count, pageSize),
                    TotalRows = filtered.Count,
                    CurrentPage = pageIndex,
                    Data = pageItems.Select(StudentMapper.ToResponse).ToList(),
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
    }
}
