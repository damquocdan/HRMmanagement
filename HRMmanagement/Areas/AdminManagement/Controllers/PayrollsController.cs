using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HRMmanagement.Models;

namespace HRMmanagement.Areas.AdminManagement.Controllers
{
    [Area("AdminManagement")]
    public class PayrollsController : Controller
    {
        private readonly HrmanagementContext _context;

        public PayrollsController(HrmanagementContext context)
        {
            _context = context;
        }

        // GET: AdminManagement/Payrolls
        public async Task<IActionResult> Index(string searchName, string filterMonth, string sortNetSalary)
        {
            var payrolls = _context.Payrolls.Include(p => p.Employee).AsQueryable();

            // Search by employee name
            if (!string.IsNullOrEmpty(searchName))
            {
                payrolls = payrolls.Where(p => p.Employee.FullName.Contains(searchName));
                ViewBag.SearchName = searchName;
            }

            // Filter by month
            if (!string.IsNullOrEmpty(filterMonth) && int.TryParse(filterMonth, out int month))
            {
                payrolls = payrolls.Where(p => p.Month == month);
                ViewBag.FilterMonth = filterMonth;
            }

            // Sort by NetSalary
            if (!string.IsNullOrEmpty(sortNetSalary))
            {
                payrolls = sortNetSalary == "asc"
                    ? payrolls.OrderBy(p => p.NetSalary)
                    : payrolls.OrderByDescending(p => p.NetSalary);
                ViewBag.SortNetSalary = sortNetSalary;
            }
            else
            {
                // Default sorting: latest records first
                payrolls = payrolls.OrderByDescending(p => p.CreatedAt);
            }

            return View(await payrolls.ToListAsync());
        }

        // POST: AdminManagement/Payrolls/CalculatePayroll
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CalculatePayroll(int month, int year)
        {
            try
            {
                // Lấy danh sách nhân viên
                var employees = await _context.Employees.Select(e => e.EmployeeId).ToListAsync();

                foreach (var employeeId in employees)
                {
                    decimal baseSalary = 0;
                    decimal bonus = 0;
                    decimal allowance = 0;
                    decimal tax = 0;
                    decimal insurance = 600000; // Mặc định
                    int lateCount = 0;
                    int leaveCount = 0;

                    // Tính BaseSalary và đếm ngày Late
                    var attendances = await _context.Attendances
                        .Where(a => a.EmployeeId == employeeId &&
                                    a.AttendanceDate.HasValue &&
                                    a.AttendanceDate.Value.Year == year &&
                                    a.AttendanceDate.Value.Month == month)
                        .ToListAsync();

                    foreach (var att in attendances)
                    {
                        if (att.Status == "Present")
                        {
                            baseSalary += 8 * 32000;
                        }
                        else if (att.Status == "Late" && att.CheckInTime.HasValue && att.CheckOutTime.HasValue)
                        {
                            var hours = (decimal)(att.CheckOutTime.Value - att.CheckInTime.Value).TotalHours;
                            baseSalary += hours * 32000;
                            lateCount++;
                        }
                        // Tính Bonus từ OvertimeHours
                        if (att.OvertimeHours.HasValue)
                        {
                            bonus += att.OvertimeHours.Value * 40000;
                        }
                    }

                    // Đếm ngày nghỉ Approved
                    leaveCount = await _context.LeaveRequests
                        .Where(l => l.EmployeeId == employeeId &&
                                    l.StartDate.HasValue &&
                                    l.StartDate.Value.Year == year &&
                                    l.StartDate.Value.Month == month)
                        .CountAsync();

                    // Tính Allowance từ Training
                    var trainingCount = await _context.Training
                        .Where(t => t.EmployeeId == employeeId &&
                                    t.Result == "Đạt" &&
                                    t.EndDate.HasValue &&
                                    t.EndDate.Value.Year == year &&
                                    t.EndDate.Value.Month == month)
                        .CountAsync();

                    allowance = trainingCount * 100000;

                    // Kiểm tra điều kiện Allowance = 0
                    if (lateCount >= 3 || leaveCount > 2)
                    {
                        allowance = 0;
                    }

                    // Tính Tax
                    tax = baseSalary * 0.1m;

                    // Tính NetSalary
                    decimal netSalary = baseSalary + bonus + allowance - tax - insurance;

                    // Kiểm tra bản ghi Payroll tồn tại
                    var existingPayroll = await _context.Payrolls
                        .FirstOrDefaultAsync(p => p.EmployeeId == employeeId &&
                                                p.Month == month &&
                                                p.Year == year);

                    if (existingPayroll != null)
                    {
                        // Cập nhật bản ghi
                        existingPayroll.BaseSalary = baseSalary;
                        existingPayroll.Bonus = bonus;
                        existingPayroll.Allowance = allowance;
                        existingPayroll.Tax = tax;
                        existingPayroll.Insurance = insurance;
                        existingPayroll.NetSalary = netSalary;
                        existingPayroll.CreatedAt = DateTime.Now;
                        _context.Payrolls.Update(existingPayroll);
                    }
                    else
                    {
                        // Tạo bản ghi mới
                        var newPayroll = new Payroll
                        {
                            EmployeeId = employeeId,
                            Month = month,
                            Year = year,
                            BaseSalary = baseSalary,
                            Bonus = bonus,
                            Allowance = allowance,
                            Tax = tax,
                            Insurance = insurance,
                            NetSalary = netSalary,
                            CreatedAt = DateTime.Now
                        };
                        _context.Payrolls.Add(newPayroll);
                    }
                }

                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = $"Tính lương tháng {month}/{year} thành công!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Lỗi khi tính lương: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: AdminManagement/Payrolls/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var payroll = await _context.Payrolls
                .Include(p => p.Employee)
                .FirstOrDefaultAsync(m => m.PayrollId == id);
            if (payroll == null)
            {
                return NotFound();
            }

            return View(payroll);
        }

        // GET: AdminManagement/Payrolls/Create
        public IActionResult Create()
        {
            ViewData["EmployeeId"] = new SelectList(_context.Employees, "EmployeeId", "FullName");
            return View();
        }

        // POST: AdminManagement/Payrolls/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PayrollId,EmployeeId,Month,Year,BaseSalary,Allowance,Bonus,Tax,Insurance,NetSalary,CreatedAt")] Payroll payroll)
        {
            if (ModelState.IsValid)
            {
                _context.Add(payroll);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["EmployeeId"] = new SelectList(_context.Employees, "EmployeeId", "FullName", payroll.EmployeeId);
            return View(payroll);
        }

        // GET: AdminManagement/Payrolls/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var payroll = await _context.Payrolls.FindAsync(id);
            if (payroll == null)
            {
                return NotFound();
            }
            ViewData["EmployeeId"] = new SelectList(_context.Employees, "EmployeeId", "FullName", payroll.EmployeeId);
            return View(payroll);
        }

        // POST: AdminManagement/Payrolls/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PayrollId,EmployeeId,Month,Year,BaseSalary,Allowance,Bonus,Tax,Insurance,NetSalary,CreatedAt")] Payroll payroll)
        {
            if (id != payroll.PayrollId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(payroll);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PayrollExists(payroll.PayrollId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["EmployeeId"] = new SelectList(_context.Employees, "EmployeeId", "FullName", payroll.EmployeeId);
            return View(payroll);
        }

        // GET: AdminManagement/Payrolls/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var payroll = await _context.Payrolls
                .Include(p => p.Employee)
                .FirstOrDefaultAsync(m => m.PayrollId == id);
            if (payroll == null)
            {
                return NotFound();
            }

            return View(payroll);
        }

        // POST: AdminManagement/Payrolls/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var payroll = await _context.Payrolls.FindAsync(id);
            if (payroll != null)
            {
                _context.Payrolls.Remove(payroll);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PayrollExists(int id)
        {
            return _context.Payrolls.Any(e => e.PayrollId == id);
        }
    }
}