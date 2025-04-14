using HRMmanagement.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HRMmanagement.Areas.AdminManagement.Controllers;

namespace HRMmanagement.Areas.AdminManagement.Controllers {

    public class DashboardController : BaseController
    {
        private readonly HrmanagementContext _context;

        public DashboardController(HrmanagementContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {

            return View();
        }
    }

}
