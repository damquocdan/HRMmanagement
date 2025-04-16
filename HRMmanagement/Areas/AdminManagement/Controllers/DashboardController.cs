using HRMmanagement.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace HRMmanagement.Areas.AdminManagement.Controllers
{
    public class DashboardController : BaseController
    {
        private readonly HrmanagementContext _context;

        public DashboardController(HrmanagementContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            // Total Employees
            ViewData["TotalEmployees"] = await _context.Employees.CountAsync();

            // Total Departments
            ViewData["TotalDepartments"] = await _context.Departments.CountAsync();

            // Pending Leave Requests
            ViewData["PendingLeaveRequests"] = await _context.LeaveRequests
                .CountAsync(lr => lr.Status == "Pending");

            // Total Payroll for Current Month
            ViewData["TotalPayrollThisMonth"] = await _context.Payrolls
                .Where(p => p.Month == DateTime.Now.Month && p.Year == DateTime.Now.Year)
                .SumAsync(p => p.NetSalary);

            // Recent Attendance (Last 5 Records)
            ViewData["RecentAttendance"] = await _context.Attendances
                .Include(a => a.Employee)
                .OrderByDescending(a => a.AttendanceDate)
                .Take(5)
                .ToListAsync();

            return View();
        }
    }
}