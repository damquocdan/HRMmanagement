using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HRMmanagement.Models;
using QRCoder;
using System.Drawing;
using System.Drawing.Imaging;
namespace HRMmanagement.Areas.AdminManagement.Controllers
{
    [Area("AdminManagement")]
    public class EmployeesController : Controller
    {
        private readonly HrmanagementContext _context;

        public EmployeesController(HrmanagementContext context)
        {
            _context = context;
        }

        // GET: AdminManagement/Employees
        public async Task<IActionResult> Index()
        {
            var hrmanagementContext = _context.Employees.Include(e => e.Department).Include(e => e.Position);
            return View(await hrmanagementContext.ToListAsync());
        }

        // GET: AdminManagement/Employees/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employee = await _context.Employees
                .Include(e => e.Department)
                .Include(e => e.Position)
                .FirstOrDefaultAsync(m => m.EmployeeId == id);
            if (employee == null)
            {
                return NotFound();
            }

            return View(employee);
        }

        // GET: AdminManagement/Employees/Create
        public IActionResult Create()
        {
            ViewData["DepartmentId"] = new SelectList(_context.Departments, "DepartmentId", "DepartmentName");
            ViewData["PositionId"] = new SelectList(_context.Positions, "PositionId", "PositionName");
            return View();
        }

        // POST: AdminManagement/Employees/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("EmployeeId,FullName,DateOfBirth,Gender,Address,Phone,Email,DepartmentId,PositionId,BaseSalary,ContractStartDate,ContractEndDate,CreatedAt")] Employee employee)
        {
            if (ModelState.IsValid)
            {
                employee.CreatedAt = DateTime.UtcNow;
                _context.Add(employee);
                await _context.SaveChangesAsync();

                try
                {
                    // Generate QR code
                    string qrContent = $"EmployeeID: {employee.EmployeeId}";
                    using (var qrGenerator = new QRCodeGenerator())
                    {
                        var qrCodeData = qrGenerator.CreateQrCode(qrContent, QRCodeGenerator.ECCLevel.Q);
                        var qrCode = new PngByteQRCode(qrCodeData);
                        byte[] qrCodeBytes = qrCode.GetGraphic(20);

                        var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "QRCodes");
                        if (!Directory.Exists(folderPath))
                        {
                            Directory.CreateDirectory(folderPath);
                        }

                        var fileName = $"qr_{employee.EmployeeId}.png";
                        var filePath = Path.Combine(folderPath, fileName);
                        await System.IO.File.WriteAllBytesAsync(filePath, qrCodeBytes);

                        employee.QrcodeData = $"/images/QRCodes/{fileName}";
                        _context.Update(employee);
                        await _context.SaveChangesAsync();
                    }
                }
                catch (Exception ex)
                {
                    TempData["Error"] = $"Failed to generate QR code: {ex.Message}";
                    // Optionally, delete the employee if QR generation is critical
                    _context.Employees.Remove(employee);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Create));
                }

                return RedirectToAction(nameof(Index));
            }

            ViewData["DepartmentId"] = new SelectList(_context.Departments, "DepartmentId", "DepartmentName", employee.DepartmentId);
            ViewData["PositionId"] = new SelectList(_context.Positions, "PositionId", "PositionName", employee.PositionId);
            return View(employee);
        }

        // GET: AdminManagement/Employees/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employee = await _context.Employees.FindAsync(id);
            if (employee == null)
            {
                return NotFound();
            }
            ViewData["DepartmentId"] = new SelectList(_context.Departments, "DepartmentId", "DepartmentId", employee.DepartmentId);
            ViewData["PositionId"] = new SelectList(_context.Positions, "PositionId", "PositionId", employee.PositionId);
            return View(employee);
        }

        // POST: AdminManagement/Employees/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("EmployeeId,FullName,DateOfBirth,Gender,Address,Phone,Email,DepartmentId,PositionId,BaseSalary,ContractStartDate,ContractEndDate,CreatedAt,QrcodeData")] Employee employee)
        {
            if (id != employee.EmployeeId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(employee);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EmployeeExists(employee.EmployeeId))
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
            ViewData["DepartmentId"] = new SelectList(_context.Departments, "DepartmentId", "DepartmentId", employee.DepartmentId);
            ViewData["PositionId"] = new SelectList(_context.Positions, "PositionId", "PositionId", employee.PositionId);
            return View(employee);
        }

        // GET: AdminManagement/Employees/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employee = await _context.Employees
                .Include(e => e.Department)
                .Include(e => e.Position)
                .FirstOrDefaultAsync(m => m.EmployeeId == id);
            if (employee == null)
            {
                return NotFound();
            }

            return View(employee);
        }

        // POST: AdminManagement/Employees/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var employee = await _context.Employees.FindAsync(id);
            if (employee != null)
            {
                _context.Employees.Remove(employee);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EmployeeExists(int id)
        {
            return _context.Employees.Any(e => e.EmployeeId == id);
        }
    }
}
