using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HRMmanagement.Models;

namespace HRMmanagement.Areas.AdminManagement.Controllers
{
    [Area("AdminManagement")]
    public class LeaveRequestsController : Controller
    {
        private readonly HrmanagementContext _context;

        public LeaveRequestsController(HrmanagementContext context)
        {
            _context = context;
        }

        // GET: AdminManagement/LeaveRequests
        public async Task<IActionResult> Index(string searchName, string searchStatus, DateOnly? searchStartDate)
        {
            var hrmanagementContext = _context.LeaveRequests
                .Include(l => l.Employee)
                .AsQueryable();

            // Search by employee name
            if (!string.IsNullOrEmpty(searchName))
            {
                hrmanagementContext = hrmanagementContext
                    .Where(l => l.Employee.FullName.Contains(searchName));
                ViewBag.SearchName = searchName;
            }

            // Search by status
            if (!string.IsNullOrEmpty(searchStatus))
            {
                hrmanagementContext = hrmanagementContext
                    .Where(l => l.Status == searchStatus);
                ViewBag.SearchStatus = searchStatus;
            }

            // Search by start date
            if (searchStartDate.HasValue)
            {
                hrmanagementContext = hrmanagementContext
                    .Where(l => l.StartDate == searchStartDate.Value);
                ViewBag.SearchStartDate = searchStartDate.Value.ToString("yyyy-MM-dd");
            }

            // Sort by CreatedAt descending
            hrmanagementContext = hrmanagementContext.OrderByDescending(l => l.CreatedAt);

            return View(await hrmanagementContext.ToListAsync());
        }

        // GET: AdminManagement/LeaveRequests/Search
        public async Task<IActionResult> Search(string searchName, string searchStatus, DateOnly? searchStartDate)
        {
            var hrmanagementContext = _context.LeaveRequests
                .Include(l => l.Employee)
                .AsQueryable();

            // Search by employee name
            if (!string.IsNullOrEmpty(searchName))
            {
                hrmanagementContext = hrmanagementContext
                    .Where(l => l.Employee.FullName.Contains(searchName));
            }

            // Search by status
            if (!string.IsNullOrEmpty(searchStatus))
            {
                hrmanagementContext = hrmanagementContext
                    .Where(l => l.Status == searchStatus);
            }

            // Search by start date
            if (searchStartDate.HasValue)
            {
                hrmanagementContext = hrmanagementContext
                    .Where(l => l.StartDate == searchStartDate.Value);
            }

            // Sort by CreatedAt descending
            hrmanagementContext = hrmanagementContext.OrderByDescending(l => l.CreatedAt);

            var results = await hrmanagementContext
                .Select(l => new
                {
                    l.LeaveId,
                    Employee = new { l.Employee.FullName },
                    l.LeaveType,
                    l.StartDate,
                    l.EndDate,
                    l.Reason,
                    l.Status,
                    l.CreatedAt
                })
                .ToListAsync();

            return Json(results);
        }

        // GET: AdminManagement/LeaveRequests/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var leaveRequest = await _context.LeaveRequests
                .Include(l => l.Employee)
                .FirstOrDefaultAsync(m => m.LeaveId == id);
            if (leaveRequest == null)
            {
                return NotFound();
            }

            return View(leaveRequest);
        }

        // GET: AdminManagement/LeaveRequests/Create
        public IActionResult Create()
        {
            ViewData["EmployeeId"] = new SelectList(_context.Employees, "EmployeeId", "EmployeeId");
            return View();
        }

        // POST: AdminManagement/LeaveRequests/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("LeaveId,EmployeeId,LeaveType,StartDate,EndDate,Reason,Status,CreatedAt")] LeaveRequest leaveRequest)
        {
            if (ModelState.IsValid)
            {
                _context.Add(leaveRequest);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["EmployeeId"] = new SelectList(_context.Employees, "EmployeeId", "EmployeeId", leaveRequest.EmployeeId);
            return View(leaveRequest);
        }

        // GET: AdminManagement/LeaveRequests/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var leaveRequest = await _context.LeaveRequests.FindAsync(id);
            if (leaveRequest == null)
            {
                return NotFound();
            }
            ViewData["EmployeeId"] = new SelectList(_context.Employees, "EmployeeId", "EmployeeId", leaveRequest.EmployeeId);
            return View(leaveRequest);
        }

        // POST: AdminManagement/LeaveRequests/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("LeaveId,EmployeeId,LeaveType,StartDate,EndDate,Reason,Status,CreatedAt")] LeaveRequest leaveRequest)
        {
            if (id != leaveRequest.LeaveId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(leaveRequest);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LeaveRequestExists(leaveRequest.LeaveId))
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
            ViewData["EmployeeId"] = new SelectList(_context.Employees, "EmployeeId", "EmployeeId", leaveRequest.EmployeeId);
            return View(leaveRequest);
        }

        // GET: AdminManagement/LeaveRequests/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var leaveRequest = await _context.LeaveRequests
                .Include(l => l.Employee)
                .FirstOrDefaultAsync(m => m.LeaveId == id);
            if (leaveRequest == null)
            {
                return NotFound();
            }

            return View(leaveRequest);
        }

        // POST: AdminManagement/LeaveRequests/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var leaveRequest = await _context.LeaveRequests.FindAsync(id);
            if (leaveRequest != null)
            {
                _context.LeaveRequests.Remove(leaveRequest);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // POST: AdminManagement/LeaveRequests/UpdateStatus
        [HttpPost]
        public async Task<IActionResult> UpdateStatus(int id, string status)
        {
            var leaveRequest = await _context.LeaveRequests.FindAsync(id);
            if (leaveRequest == null)
            {
                return Json(new { success = false, message = "Không tìm thấy đơn xin nghỉ." });
            }

            leaveRequest.Status = status;
            try
            {
                await _context.SaveChangesAsync();
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        private bool LeaveRequestExists(int id)
        {
            return _context.LeaveRequests.Any(e => e.LeaveId == id);
        }
    }
}