using MayNghien.Infrastructures.Models.Responses;
using Microsoft.EntityFrameworkCore;
using VAcademyConnect.Application.Interfaces;
using VAcademyConnect.Application.Interfaces.Repositories;
using VAcademyConnect.Application.Interfaces.TuitionFees;
using VAcademyConnect.Application.Mapping;
using VAcademyConnect.DTOs.Requests;
using VAcademyConnect.DTOs.Responses;
using VAcademyConnect.Domain.Entities;

namespace VAcademyConnect.Application.Implementations.TuitionFees
{
    // Ước tính hạn đóng tiền dựa trên nhịp điểm danh thực tế của học sinh:
    // - Chuẩn: 12 buổi/tháng => 3 buổi/tuần => đủ 12 buổi sau 4 tuần (hạn = ngày 1 tháng sau).
    // - Có 1 buổi nghỉ phép/tuần (không tính tiền buổi đó) => chỉ còn 2 buổi/tuần được tính
    //   => cần 6 tuần mới đủ 12 buổi (hạn ~ ngày 15 tháng sau nữa).
    public class EstimateTuitionFeeHandler : IEstimateTuitionFeeHandler
    {
        private const int SessionsPerCycle = 12;
        private const int StandardSessionsPerWeek = 3;

        private readonly IAttendanceRepository _attendanceRepository;
        private readonly IClassRepository _classRepository;
        private readonly ITuitionFeeRepository _tuitionFeeRepository;
        private readonly ICurrentUserService _currentUserService;

        public EstimateTuitionFeeHandler(IAttendanceRepository attendanceRepository, IClassRepository classRepository,
            ITuitionFeeRepository tuitionFeeRepository, ICurrentUserService currentUserService)
        {
            _attendanceRepository = attendanceRepository;
            _classRepository = classRepository;
            _tuitionFeeRepository = tuitionFeeRepository;
            _currentUserService = currentUserService;
        }

        public async Task<AppResponse<TuitionFeeResponse>> HandleAsync(TuitionFeeEstimateRequest request)
        {
            var result = new AppResponse<TuitionFeeResponse>();
            try
            {
                var user = await _currentUserService.GetCurrentUserAsync();
                if (user == null)
                    return result.BuildError("Unauthorize.");

                var classEntity = await _classRepository.GetAsync(request.ClassId!.Value);
                if (classEntity == null || classEntity.IsDeleted)
                    return result.BuildError("Class was not found or has been deleted.");

                var cycleStart = request.CycleStartDate.Date;
                var today = DateTime.UtcNow.Date;

                var attendances = await _attendanceRepository.FindByAsync(a => !a.IsDeleted &&
                    a.StudentId == request.StudentId && a.ClassId == request.ClassId &&
                    a.Date >= cycleStart && a.Date <= today).ToListAsync();

                var excusedCount = attendances.Count(a => !a.CountAsPaid);
                var weeksElapsed = Math.Max(1, (int)Math.Ceiling((today - cycleStart).TotalDays / 7));
                var excusedPerWeek = (int)Math.Round((double)excusedCount / weeksElapsed, MidpointRounding.AwayFromZero);

                var effectiveSessionsPerWeek = Math.Max(1, StandardSessionsPerWeek - excusedPerWeek);
                var weeksNeeded = (int)Math.Ceiling((double)SessionsPerCycle / effectiveSessionsPerWeek);
                var estimatedDueDate = cycleStart.AddDays(weeksNeeded * 7);
                var month = cycleStart.ToString("yyyy-MM");

                var existing = await _tuitionFeeRepository.FindByAsync(x =>
                        x.StudentId == request.StudentId && x.Month == month && !x.IsDeleted)
                    .Include(x => x.Student).FirstOrDefaultAsync();

                TuitionFee saved;
                if (existing != null)
                {
                    existing.Amount = classEntity.Tuition;
                    existing.DueDate = estimatedDueDate;
                    existing.ModifiedBy = user.Email;
                    existing.ModifiedOn = DateTime.UtcNow;
                    await _tuitionFeeRepository.EditAsync(existing);
                    saved = existing;
                }
                else
                {
                    var newFee = new TuitionFee
                    {
                        Id = Guid.NewGuid(),
                        StudentId = request.StudentId,
                        Month = month,
                        Amount = classEntity.Tuition,
                        DueDate = estimatedDueDate,
                        CreatedBy = user.Email,
                        CreatedOn = DateTime.UtcNow,
                    };
                    await _tuitionFeeRepository.AddAsync(newFee);

                    saved = await _tuitionFeeRepository.FindByAsync(x => x.Id == newFee.Id)
                        .Include(x => x.Student).FirstOrDefaultAsync() ?? newFee;
                }

                var response = TuitionFeeMapper.ToResponse(saved);
                result.BuildResult(response, $"Estimated due date: {estimatedDueDate:yyyy-MM-dd}.");
            }
            catch (Exception ex)
            {
                result.BuildError(ex.Message + " " + ex.StackTrace);
            }
            return result;
        }
    }
}
