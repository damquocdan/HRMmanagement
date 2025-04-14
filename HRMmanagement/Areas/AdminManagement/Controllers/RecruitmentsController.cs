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
    public class RecruitmentsController : Controller
    {
        private readonly HrmanagementContext _context;

        public RecruitmentsController(HrmanagementContext context)
        {
            _context = context;
        }

        // GET: AdminManagement/Recruitments
        public async Task<IActionResult> Index()
        {
            var hrmanagementContext = _context.Recruitments.Include(r => r.AppliedPosition);
            return View(await hrmanagementContext.ToListAsync());
        }

        // GET: AdminManagement/Recruitments/Details/5
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

        // GET: AdminManagement/Recruitments/Create
        public IActionResult Create()
        {
            ViewData["AppliedPositionId"] = new SelectList(_context.Positions, "PositionId", "PositionId");
            return View();
        }

        // POST: AdminManagement/Recruitments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CandidateId,FullName,Email,Phone,AppliedPositionId,Cvpath,InterviewDate,Status,CreatedAt")] Recruitment recruitment)
        {
            if (ModelState.IsValid)
            {
                _context.Add(recruitment);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["AppliedPositionId"] = new SelectList(_context.Positions, "PositionId", "PositionId", recruitment.AppliedPositionId);
            return View(recruitment);
        }

        // GET: AdminManagement/Recruitments/Edit/5
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

        // POST: AdminManagement/Recruitments/Edit/5
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

        // GET: AdminManagement/Recruitments/Delete/5
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

        // POST: AdminManagement/Recruitments/Delete/5
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
