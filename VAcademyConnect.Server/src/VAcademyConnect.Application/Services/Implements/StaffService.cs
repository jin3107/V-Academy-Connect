using LinqKit;
using MayNghien.Infrastructures.Models.Requests;
using MayNghien.Infrastructures.Models.Responses;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using VAcademyConnect.Commons.Enums;
using VAcademyConnect.DTOs.Authentication.Requests;
using VAcademyConnect.DTOs.Authentication.Responses;
using VAcademyConnect.DTOs.Requests;
using VAcademyConnect.DTOs.Responses;
using VAcademyConnect.Domain.Entities;
using VAcademyConnect.Application.Interfaces;
using VAcademyConnect.Application.Interfaces.Repositories;
using VAcademyConnect.Application.Interfaces.Services;
using VAcademyConnect.Application.Services.Mapping;
using static MayNghien.Infrastructures.Helpers.SearchHelper;

namespace VAcademyConnect.Application.Services.Implements
{
    public class StaffService : IStaffService
    {
        private readonly IStaffRepository _staffRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IUserAccountService _userAccountService;

        public StaffService(IStaffRepository staffRepository, ICurrentUserService currentUserService,
            IUserAccountService userAccountService)
        {
            _staffRepository = staffRepository;
            _currentUserService = currentUserService;
            _userAccountService = userAccountService;
        }

        public async Task<AppResponse<StaffResponse>> CreateAsync(StaffRequest request)
        {
            var result = new AppResponse<StaffResponse>();
            try
            {
                var currentUser = await _currentUserService.GetCurrentUserAsync();
                if (currentUser == null)
                    return result.BuildError("Unauthorized");

                if (request.Role.Equals("Teacher", StringComparison.OrdinalIgnoreCase))
                    return result.BuildError("Cannot create Teacher as Staff. Teacher role is assigned to TenantAdmin only.");

                var createStaff = await CreateStaff(request);
                if (createStaff == null)
                    return result.BuildError("Failed to create staff.");

                var (createSucceeded, createErrors, createdUser) = await CreateUser(request);
                if (!createSucceeded)
                {
                    await _staffRepository.DeleteAsync(createStaff);

                    if (createdUser != null)
                        await _userAccountService.DeleteAsync(createdUser);

                    return result.BuildError("Cannot create user: " + string.Join(", ", createErrors));
                }

                var identityUser = await _userAccountService.FindByEmailAsync(request.Email);
                if (identityUser == null)
                    return result.BuildError("Failed to retrieve created Staff.");

                await _userAccountService.AssignRoleAsync(identityUser, request.Role);
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Email, request.Email),
                    new Claim(ClaimTypes.Role, request.Role),
                };

                var response = StaffMapper.ToResponse(createStaff);
                return result.BuildResult(response, "Staff created successfully.");
            }
            catch (Exception ex)
            {
                return result.BuildError(ex.Message + " " + ex.StackTrace);
            }
        }

        private async Task<(bool Succeeded, IEnumerable<string> Errors, ApplicationUser? User)> CreateUser(StaffRequest request)
        {
            var user = await _currentUserService.GetCurrentUserAsync();
            if (request == null)
                throw new ArgumentNullException(nameof(request), "Register request for User is invalid");

            var identityUser = new ApplicationUser
            {
                FullName = request.Name,
                UserName = request.Email,
                Email = request.Email,
                TenantId = user!.TenantId,
                EmailConfirmed = true,
                SecurityStamp = Guid.NewGuid().ToString(),
                PhoneNumber = request.PhoneNumber,
                Role = Role.Staff
            };

            return await _userAccountService.CreateAsync(identityUser, request.Password);
        }

        public async Task<AppResponse<string>> DeleteAsync(Guid id)
        {
            var result = new AppResponse<string>();
            try
            {
                var currentUser = await _currentUserService.GetCurrentUserAsync();
                if (currentUser == null)
                    return result.BuildError("Unauthorized");

                var staff = await _staffRepository.GetAsync(id);
                if (staff == null)
                    return result.BuildError("Staff not found");

                staff.IsDeleted = true;
                staff.ModifiedBy = currentUser.Email;
                staff.ModifiedOn = DateTime.UtcNow;

                await _staffRepository.EditAsync(staff);
                return result.BuildResult("Successfully deleted", "Staff deleted successfully");
            }
            catch (Exception ex)
            {
                return result.BuildError(ex.Message + " " + ex.StackTrace);
            }
        }

        public async Task<AppResponse<StaffResponse>> GetByIdAsync(Guid id)
        {
            var result = new AppResponse<StaffResponse>();
            try
            {
                var currentUser = await _currentUserService.GetCurrentUserAsync();
                if (currentUser == null)
                    return result.BuildError("Unauthorized");

                var staff = await _staffRepository.GetAsync(id);
                if (staff == null || staff.IsDeleted)
                    return result.BuildError("Staff not found");

                var response = StaffMapper.ToResponse(staff);
                return result.BuildResult(response);
            }
            catch (Exception ex)
            {
                return result.BuildError(ex.Message + " " + ex.StackTrace);
            }
        }

        public async Task<AppResponse<StaffResponse>> UpdateAsync(StaffRequest request)
        {
            var result = new AppResponse<StaffResponse>();
            try
            {
                var currentUser = await _currentUserService.GetCurrentUserAsync();
                if (currentUser == null)
                    return result.BuildError("Unauthorized");

                if (request?.Id == null || request.Id == new Guid())
                    return result.BuildError("Staff ID is required");

                var staff = await _staffRepository.GetAsync(request.Id);
                if (staff == null || staff.IsDeleted)
                    return result.BuildError("Staff not found");

                if ((staff.Email != request.Email || staff.PhoneNumber != request.PhoneNumber) &&
                    await _userAccountService.ExistsByEmailOrPhoneAsync(request.Email, request.PhoneNumber))
                    return result.BuildError("Email or phone number already in use");

                var currentEmail = staff.Email;
                staff.Name = request.Name;
                staff.Email = request.Email;
                staff.PhoneNumber = request.PhoneNumber;
                staff.DateOfBirth = request.DateOfBirth;
                staff.Gender = request.Gender;
                staff.Address = request.Address;
                staff.Role = request.Role;
                staff.ModifiedBy = currentUser.Email;
                staff.ModifiedOn = DateTime.UtcNow;

                var identityUser = await _userAccountService.FindByEmailAsync(currentEmail);
                if (identityUser == null)
                    return result.BuildError("Associated user account not found");

                identityUser.Email = request.Email;
                identityUser.PhoneNumber = request.PhoneNumber;

                var (updateSucceeded, updateErrors) = await _userAccountService.UpdateAsync(identityUser);
                if (!updateSucceeded)
                    return result.BuildError("Failed to update user: " + string.Join(", ", updateErrors));

                await _userAccountService.AssignRoleAsync(identityUser, request.Role);
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Email, request.Email),
                    new Claim(ClaimTypes.Role, request.Role),
                };

                await _staffRepository.EditAsync(staff);
                var response = StaffMapper.ToResponse(staff);

                return result.BuildResult(response, "Staff updated successfully");
            }
            catch (Exception ex)
            {
                return result.BuildError(ex.Message + " " + ex.StackTrace);
            }
        }

        private async Task<Staff> CreateStaff(StaffRequest request)
        {
            var currentUser = await _currentUserService.GetCurrentUserAsync();
            if (request == null)
                throw new ArgumentNullException(nameof(request), "Request for Staff is invalid");

            var staff = StaffMapper.ToEntity(request);
            staff.Id = Guid.NewGuid();
            staff.CreatedBy = currentUser?.Email;
            staff.CreatedOn = DateTime.UtcNow;
            staff.TenantId = currentUser?.TenantId;
            staff.IsDeleted = false;

            await _staffRepository.AddAsync(staff);
            return staff;
        }

        public async Task<AppResponse<SearchResponse<StaffResponse>>> SearchAsync(SearchRequest request)
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
