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
    public class PerformanceEvaluationsController : Controller
    {
        private readonly HrmanagementContext _context;

        public PerformanceEvaluationsController(HrmanagementContext context)
        {
            _context = context;
        }

        // GET: AdminManagement/PerformanceEvaluations
        public async Task<IActionResult> Index()
        {
            var hrmanagementContext = _context.PerformanceEvaluations.Include(p => p.Employee).Include(p => p.Evaluator);
            return View(await hrmanagementContext.ToListAsync());
        }

        // GET: AdminManagement/PerformanceEvaluations/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var performanceEvaluation = await _context.PerformanceEvaluations
                .Include(p => p.Employee)
                .Include(p => p.Evaluator)
                .FirstOrDefaultAsync(m => m.EvaluationId == id);
            if (performanceEvaluation == null)
            {
                return NotFound();
            }

            return View(performanceEvaluation);
        }

        // GET: AdminManagement/PerformanceEvaluations/Create
        public IActionResult Create()
        {
            ViewData["EmployeeId"] = new SelectList(_context.Employees, "EmployeeId", "EmployeeId");
            ViewData["EvaluatorId"] = new SelectList(_context.Employees, "EmployeeId", "EmployeeId");
            return View();
        }

        // POST: AdminManagement/PerformanceEvaluations/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("EvaluationId,EmployeeId,EvaluationDate,EvaluatorId,SelfScore,ManagerScore,PeerScore,Comments,CreatedAt")] PerformanceEvaluation performanceEvaluation)
        {
            if (ModelState.IsValid)
            {
                _context.Add(performanceEvaluation);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["EmployeeId"] = new SelectList(_context.Employees, "EmployeeId", "EmployeeId", performanceEvaluation.EmployeeId);
            ViewData["EvaluatorId"] = new SelectList(_context.Employees, "EmployeeId", "EmployeeId", performanceEvaluation.EvaluatorId);
            return View(performanceEvaluation);
        }

        // GET: AdminManagement/PerformanceEvaluations/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var performanceEvaluation = await _context.PerformanceEvaluations.FindAsync(id);
            if (performanceEvaluation == null)
            {
                return NotFound();
            }
            ViewData["EmployeeId"] = new SelectList(_context.Employees, "EmployeeId", "EmployeeId", performanceEvaluation.EmployeeId);
            ViewData["EvaluatorId"] = new SelectList(_context.Employees, "EmployeeId", "EmployeeId", performanceEvaluation.EvaluatorId);
            return View(performanceEvaluation);
        }

        // POST: AdminManagement/PerformanceEvaluations/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("EvaluationId,EmployeeId,EvaluationDate,EvaluatorId,SelfScore,ManagerScore,PeerScore,Comments,CreatedAt")] PerformanceEvaluation performanceEvaluation)
        {
            if (id != performanceEvaluation.EvaluationId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(performanceEvaluation);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PerformanceEvaluationExists(performanceEvaluation.EvaluationId))
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
            ViewData["EmployeeId"] = new SelectList(_context.Employees, "EmployeeId", "EmployeeId", performanceEvaluation.EmployeeId);
            ViewData["EvaluatorId"] = new SelectList(_context.Employees, "EmployeeId", "EmployeeId", performanceEvaluation.EvaluatorId);
            return View(performanceEvaluation);
        }

        // GET: AdminManagement/PerformanceEvaluations/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var performanceEvaluation = await _context.PerformanceEvaluations
                .Include(p => p.Employee)
                .Include(p => p.Evaluator)
                .FirstOrDefaultAsync(m => m.EvaluationId == id);
            if (performanceEvaluation == null)
            {
                return NotFound();
            }

            return View(performanceEvaluation);
        }

        // POST: AdminManagement/PerformanceEvaluations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var performanceEvaluation = await _context.PerformanceEvaluations.FindAsync(id);
            if (performanceEvaluation != null)
            {
                _context.PerformanceEvaluations.Remove(performanceEvaluation);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PerformanceEvaluationExists(int id)
        {
            return _context.PerformanceEvaluations.Any(e => e.EvaluationId == id);
        }
    }
}
