using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HRMmanagement.Models;
using Microsoft.IdentityModel.Tokens;

namespace HRMmanagement.Controllers
{
    public class AttendancesController : Controller
    {
        private readonly HrmanagementContext _context;
        private readonly TimeSpan StandardWorkHours = TimeSpan.FromHours(8);
        private readonly TimeSpan LateThreshold = new TimeSpan(8, 30, 0); // 8:30 AM
        public AttendancesController(HrmanagementContext context)
        {
            _context = context;
        }

        // GET: Attendances
        public async Task<IActionResult> Index()
        {
            var hrmanagementContext = _context.Attendances.Include(a => a.Employee);
            return View(await hrmanagementContext.ToListAsync());
        }

        // GET: Attendances/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Attendances/ScanAttendance
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ScanAttendance([Bind("EmployeeId")] Attendance attendance)
        {
            if (!ModelState.IsValid || !attendance.EmployeeId.HasValue)
            {
                TempData["Error"] = "Invalid employee data.";
                return RedirectToAction(nameof(Create));
            }

            var employee = await _context.Employees
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.EmployeeId == attendance.EmployeeId.Value);
            if (employee == null)
            {
                TempData["Error"] = "Employee not found.";
                return RedirectToAction(nameof(Create));
            }

            var today = DateOnly.FromDateTime(DateTime.UtcNow); // Use UTC for consistency
            var currentTime = DateTime.UtcNow;
            var existingAttendance = await _context.Attendances
                .FirstOrDefaultAsync(a => a.EmployeeId == attendance.EmployeeId && a.AttendanceDate == today);

            if (existingAttendance == null)
            {
                var newAttendance = new Attendance
                {
                    EmployeeId = attendance.EmployeeId,
                    AttendanceDate = today,
                    CheckInTime = DateTime.UtcNow,
                    Status = currentTime.TimeOfDay > LateThreshold ? "Late" : "Present",
                    CreatedAt = DateTime.UtcNow
                };
                _context.Add(newAttendance);
                TempData["Success"] = $"Chấm công vào cho {employee.FullName} (ID: {employee.EmployeeId}).";
            }
            else if (existingAttendance.CheckOutTime == null)
            {
                existingAttendance.CheckOutTime = DateTime.UtcNow;

                if (existingAttendance.CheckInTime.HasValue && existingAttendance.CheckOutTime.HasValue)
                {
                    var workDuration = existingAttendance.CheckOutTime.Value - existingAttendance.CheckInTime.Value;

                    // Calculate overtime
                    existingAttendance.OvertimeHours = workDuration > StandardWorkHours
                        ? (decimal)(workDuration - StandardWorkHours).TotalHours
                        : 0;

                    // Update status based on work hours
                    if (workDuration.TotalHours < 8)
                    {
                        existingAttendance.Status = "Late"; // Insufficient hours considered Late
                    }
                    else
                    {
                        existingAttendance.Status = existingAttendance.Status == "Late"
                            ? "Late"
                            : "Present"; // Maintain Late if already Late, else Present
                    }
                }

                _context.Update(existingAttendance);
                TempData["Success"] = $"Chấm công ra cho {employee.FullName} (ID: {employee.EmployeeId}).";
            }
            else
            {
                TempData["Error"] = "Việc chấm công hôm nay đã hoàn tất.";
                return RedirectToAction(nameof(Create));
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                TempData["Error"] = "Concurrency error occurred. Please try again.";
                return RedirectToAction(nameof(Create));
            }

            return RedirectToAction(nameof(Create));
        }

        // Other actions (Details, Edit, Delete) remain unchanged
        // ...

        private bool AttendanceExists(int id)
        {
            return _context.Attendances.Any(e => e.AttendanceId == id);
        }
    }
}