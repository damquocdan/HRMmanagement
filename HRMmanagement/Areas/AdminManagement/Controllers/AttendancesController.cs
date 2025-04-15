using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HRMmanagement.Models;
using ClosedXML.Excel;
using System.IO;
using System.Text;

namespace HRMmanagement.Areas.AdminManagement.Controllers
{
    [Area("AdminManagement")]
    public class AttendancesController : Controller
    {
        private readonly HrmanagementContext _context;

        public AttendancesController(HrmanagementContext context)
        {
            _context = context;
        }

        // GET: AdminManagement/Attendances
        public async Task<IActionResult> Index(
            string searchName,
            string status,
            string attendanceDate,
            string createdAtFrom,
            string createdAtTo,
            int? createdAtMonth)
        {
            // Store filter values in ViewBag to persist in the view
            ViewBag.SearchName = searchName;
            ViewBag.Status = status;
            ViewBag.AttendanceDate = attendanceDate;
            ViewBag.CreatedAtFrom = createdAtFrom;
            ViewBag.CreatedAtTo = createdAtTo;
            ViewBag.CreatedAtMonth = createdAtMonth?.ToString();

            // Parse date inputs
            DateOnly? parsedAttendanceDate = string.IsNullOrEmpty(attendanceDate) ? null : DateOnly.Parse(attendanceDate);
            DateTime? parsedCreatedAtFrom = string.IsNullOrEmpty(createdAtFrom) ? null : DateTime.Parse(createdAtFrom);
            DateTime? parsedCreatedAtTo = string.IsNullOrEmpty(createdAtTo) ? null : DateTime.Parse(createdAtTo);

            // Query with includes
            var query = _context.Attendances
                .Include(a => a.Employee)
                .AsQueryable();

            // Apply filters
            if (!string.IsNullOrEmpty(searchName))
            {
                query = query.Where(a => a.Employee != null && a.Employee.FullName.Contains(searchName));
            }

            if (!string.IsNullOrEmpty(status))
            {
                query = query.Where(a => a.Status == status);
            }

            if (parsedAttendanceDate.HasValue)
            {
                query = query.Where(a => a.AttendanceDate == parsedAttendanceDate.Value);
            }

            if (parsedCreatedAtFrom.HasValue)
            {
                query = query.Where(a => a.CreatedAt.HasValue && a.CreatedAt.Value >= parsedCreatedAtFrom.Value);
            }

            if (parsedCreatedAtTo.HasValue)
            {
                query = query.Where(a => a.CreatedAt.HasValue && a.CreatedAt.Value <= parsedCreatedAtTo.Value);
            }

            if (createdAtMonth.HasValue)
            {
                query = query.Where(a => a.CreatedAt.HasValue && a.CreatedAt.Value.Month == createdAtMonth.Value);
            }

            // Sort by CreatedAt descending (newest first)
            query = query.OrderByDescending(a => a.CreatedAt);

            return View(await query.ToListAsync());
        }

        // GET: AdminManagement/Attendances/GetFilteredAttendances
        public async Task<IActionResult> GetFilteredAttendances(
            string searchName,
            string status,
            string attendanceDate,
            string createdAtFrom,
            string createdAtTo,
            int? createdAtMonth)
        {
            // Parse date inputs
            DateOnly? parsedAttendanceDate = string.IsNullOrEmpty(attendanceDate) ? null : DateOnly.Parse(attendanceDate);
            DateTime? parsedCreatedAtFrom = string.IsNullOrEmpty(createdAtFrom) ? null : DateTime.Parse(createdAtFrom);
            DateTime? parsedCreatedAtTo = string.IsNullOrEmpty(createdAtTo) ? null : DateTime.Parse(createdAtTo);

            // Query with includes
            var query = _context.Attendances
                .Include(a => a.Employee)
                .AsQueryable();

            // Apply filters
            if (!string.IsNullOrEmpty(searchName))
            {
                query = query.Where(a => a.Employee != null && a.Employee.FullName.Contains(searchName));
            }

            if (!string.IsNullOrEmpty(status))
            {
                query = query.Where(a => a.Status == status);
            }

            if (parsedAttendanceDate.HasValue)
            {
                query = query.Where(a => a.AttendanceDate == parsedAttendanceDate.Value);
            }

            if (parsedCreatedAtFrom.HasValue)
            {
                query = query.Where(a => a.CreatedAt.HasValue && a.CreatedAt.Value >= parsedCreatedAtFrom.Value);
            }

            if (parsedCreatedAtTo.HasValue)
            {
                query = query.Where(a => a.CreatedAt.HasValue && a.CreatedAt.Value <= parsedCreatedAtTo.Value);
            }

            if (createdAtMonth.HasValue)
            {
                query = query.Where(a => a.CreatedAt.HasValue && a.CreatedAt.Value.Month == createdAtMonth.Value);
            }

            // Sort by CreatedAt descending (newest first)
            query = query.OrderByDescending(a => a.CreatedAt);

            var attendances = await query.ToListAsync();

            // Generate HTML for table rows
            var html = new StringBuilder();
            foreach (var item in attendances)
            {
                html.Append("<tr>");
                html.Append($"<td>{(item.Employee?.FullName ?? "N/A")}</td>");
                html.Append($"<td>{(item.AttendanceDate.HasValue ? item.AttendanceDate.Value.ToString("dd/MM/yyyy") : "")}</td>");
                html.Append($"<td>{(item.CheckInTime.HasValue ? item.CheckInTime.Value.ToString("HH:mm") : "")}</td>");
                html.Append($"<td>{(item.CheckOutTime.HasValue ? item.CheckOutTime.Value.ToString("HH:mm") : "")}</td>");
                html.Append($"<td>{(item.OvertimeHours.HasValue ? item.OvertimeHours.Value.ToString("F2") : "")}</td>");
                html.Append($"<td>{(item.Status == "Present" ? "Đầy đủ" :
      item.Status == "Late" ? "Muộn" : "")}</td>");
                html.Append($"<td>{(item.CreatedAt.HasValue ? item.CreatedAt.Value.ToString("dd/MM/yyyy HH:mm") : "")}</td>");
                html.Append("<td>");
                html.Append($"<a href=\"/AdminManagement/Attendances/Details/{item.AttendanceId}\" class=\"btn btn-sm btn-secondary\" style=\"background-color: #213d86; border-color: #213d86;\"><i class=\"fa fa-eye\"></i> Chi tiết</a> ");
                html.Append($"<a href=\"/AdminManagement/Attendances/Delete/{item.AttendanceId}\" class=\"btn btn-sm btn-danger\" style=\"background-color: #f7941c; border-color: #f7941c;\"><i class=\"fa fa-trash\"></i> Xóa</a>");
                html.Append("</td>");
                html.Append("</tr>");
            }

            return Content(html.ToString(), "text/html");
        }

        // GET: AdminManagement/Attendances/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var attendance = await _context.Attendances
                .Include(a => a.Employee)
                .FirstOrDefaultAsync(m => m.AttendanceId == id);
            if (attendance == null)
            {
                return NotFound();
            }

            return View(attendance);
        }

        // GET: AdminManagement/Attendances/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var attendance = await _context.Attendances
                .Include(a => a.Employee)
                .FirstOrDefaultAsync(m => m.AttendanceId == id);
            if (attendance == null)
            {
                return NotFound();
            }

            return View(attendance);
        }

        // POST: AdminManagement/Attendances/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var attendance = await _context.Attendances.FindAsync(id);
            if (attendance != null)
            {
                _context.Attendances.Remove(attendance);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: AdminManagement/Attendances/ExportToExcel
        public async Task<IActionResult> ExportToExcel(
            string searchName,
            string status,
            string attendanceDate,
            string createdAtFrom,
            string createdAtTo,
            int? createdAtMonth)
        {
            // Parse date inputs
            DateOnly? parsedAttendanceDate = string.IsNullOrEmpty(attendanceDate) ? null : DateOnly.Parse(attendanceDate);
            DateTime? parsedCreatedAtFrom = string.IsNullOrEmpty(createdAtFrom) ? null : DateTime.Parse(createdAtFrom);
            DateTime? parsedCreatedAtTo = string.IsNullOrEmpty(createdAtTo) ? null : DateTime.Parse(createdAtTo);

            // Apply the same filters as in Index
            var query = _context.Attendances
                .Include(a => a.Employee)
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchName))
            {
                query = query.Where(a => a.Employee != null && a.Employee.FullName.Contains(searchName));
            }

            if (!string.IsNullOrEmpty(status))
            {
                query = query.Where(a => a.Status == status);
            }

            if (parsedAttendanceDate.HasValue)
            {
                query = query.Where(a => a.AttendanceDate == parsedAttendanceDate.Value);
            }

            if (parsedCreatedAtFrom.HasValue || parsedCreatedAtTo.HasValue)
            {
                query = query.Where(a =>
                    a.CreatedAt.HasValue &&
                    (!parsedCreatedAtFrom.HasValue || a.CreatedAt.Value.Date >= parsedCreatedAtFrom.Value.Date) &&
                    (!parsedCreatedAtTo.HasValue || a.CreatedAt.Value.Date <= parsedCreatedAtTo.Value.Date)
                );
            }

            if (createdAtMonth.HasValue)
            {
                query = query.Where(a => a.CreatedAt.HasValue && a.CreatedAt.Value.Month == createdAtMonth.Value);
            }

            var attendances = await query.OrderByDescending(a => a.CreatedAt).ToListAsync();

            // Create Excel file
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Attendances");
                var currentRow = 1;

                // Headers
                worksheet.Cell(currentRow, 1).Value = "Họ và tên";
                worksheet.Cell(currentRow, 2).Value = "Ngày chấm công";
                worksheet.Cell(currentRow, 3).Value = "Giờ vào";
                worksheet.Cell(currentRow, 4).Value = "Giờ ra";
                worksheet.Cell(currentRow, 5).Value = "Giờ tăng ca";
                worksheet.Cell(currentRow, 6).Value = "Trạng thái";
                worksheet.Cell(currentRow, 7).Value = "Ngày tạo";

                // Data
                foreach (var item in attendances)
                {
                    currentRow++;
                    worksheet.Cell(currentRow, 1).Value = item.Employee?.FullName ?? "N/A";
                    worksheet.Cell(currentRow, 2).Value = item.AttendanceDate.HasValue ? item.AttendanceDate.Value.ToString("dd/MM/yyyy") : "";
                    worksheet.Cell(currentRow, 3).Value = item.CheckInTime.HasValue ? item.CheckInTime.Value.ToString("HH:mm") : "";
                    worksheet.Cell(currentRow, 4).Value = item.CheckOutTime.HasValue ? item.CheckOutTime.Value.ToString("HH:mm") : "";
                    worksheet.Cell(currentRow, 5).Value = item.OvertimeHours.HasValue ? item.OvertimeHours.Value.ToString("F2") : "";
                    worksheet.Cell(currentRow, 6).Value = item.Status ?? "";
                    worksheet.Cell(currentRow, 7).Value = item.CreatedAt.HasValue ? item.CreatedAt.Value.ToString("dd/MM/yyyy HH:mm") : "";
                }

                // Auto-fit columns
                worksheet.Columns().AdjustToContents();

                // Export to memory stream
                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();
                    return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Attendances.xlsx");
                }
            }
        }

        private bool AttendanceExists(int id)
        {
            return _context.Attendances.Any(e => e.AttendanceId == id);
        }
    }
}