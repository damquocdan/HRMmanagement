using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HRMmanagement.Models;

namespace HRMmanagement.Controllers
{
    public class RecruitmentsController : Controller
    {
        private readonly HrmanagementContext _context;

        public RecruitmentsController(HrmanagementContext context)
        {
            _context = context;
        }

        // GET: Recruitments
        public async Task<IActionResult> Index()
        {
            var hrmanagementContext = _context.Recruitments.Include(r => r.AppliedPosition);
            return View(await hrmanagementContext.ToListAsync());
        }

        // GET: Recruitments/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var recruitment = await _context.Recruitments
                .Include(r => r.AppliedPosition)
                .FirstOrDefaultAsync(m => m.CandidateId == id);
            if (recruitment == null)
            {
                return NotFound();
            }

            return View(recruitment);
        }
        [HttpPost]
        public IActionResult FromHome()
        {
            string name = Request.Form["name"];
            string email = Request.Form["email"];
            string description = Request.Form["description"];

            TempData["Name"] = name;
            TempData["Email"] = email;
            TempData["Description"] = description;

            return RedirectToAction("Create");
        }

        // GET: Recruitments/Create
        public IActionResult Create()
        {
            ViewData["AppliedPositionId"] = new SelectList(_context.Positions, "PositionId", "PositionName");
            return View();
        }

        // POST: Recruitments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("RecruitmentId,FullName,Email,Phone,AppliedPositionId,InterviewDate,Status")] Recruitment recruitment, IFormFile CvFile)
        {
            if (ModelState.IsValid)
            {
                if (CvFile != null && CvFile.Length > 0)
                {
                    var fileName = Path.GetFileName(CvFile.FileName);
                    var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads/cv");
                    Directory.CreateDirectory(uploadPath); // Tạo thư mục nếu chưa có

                    var filePath = Path.Combine(uploadPath, fileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await CvFile.CopyToAsync(stream);
                    }

                    recruitment.Cvpath = "/uploads/cv/" + fileName;
                }

                recruitment.Status = "Chờ xử lý"; // Gán trạng thái mặc định

                _context.Add(recruitment);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Ứng viên đã được thêm thành công!";
                ModelState.Clear(); // Reset form

                // Trả về form rỗng
                ViewData["AppliedPositionId"] = new SelectList(_context.Positions, "PositionId", "PositionName");
                return View();
            }

            // Nếu có lỗi thì giữ nguyên dữ liệu nhập lại
            ViewData["AppliedPositionId"] = new SelectList(_context.Positions, "PositionId", "PositionName", recruitment.AppliedPositionId);
            return View(recruitment);
        }



        // GET: Recruitments/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var recruitment = await _context.Recruitments.FindAsync(id);
            if (recruitment == null)
            {
                return NotFound();
            }
            ViewData["AppliedPositionId"] = new SelectList(_context.Positions, "PositionId", "PositionId", recruitment.AppliedPositionId);
            return View(recruitment);
        }

        // POST: Recruitments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CandidateId,FullName,Email,Phone,AppliedPositionId,Cvpath,InterviewDate,Status,CreatedAt")] Recruitment recruitment)
        {
            if (id != recruitment.CandidateId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(recruitment);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RecruitmentExists(recruitment.CandidateId))
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
            ViewData["AppliedPositionId"] = new SelectList(_context.Positions, "PositionId", "PositionId", recruitment.AppliedPositionId);
            return View(recruitment);
        }

        // GET: Recruitments/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var recruitment = await _context.Recruitments
                .Include(r => r.AppliedPosition)
                .FirstOrDefaultAsync(m => m.CandidateId == id);
            if (recruitment == null)
            {
                return NotFound();
            }

            return View(recruitment);
        }

        // POST: Recruitments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var recruitment = await _context.Recruitments.FindAsync(id);
            if (recruitment != null)
            {
                _context.Recruitments.Remove(recruitment);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool RecruitmentExists(int id)
        {
            return _context.Recruitments.Any(e => e.CandidateId == id);
        }
    }
}
