using System;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HRMmanagement.Models;
using Newtonsoft.Json;

namespace HRMmanagement.Areas.AdminManagement.Controllers
{
    [Area("AdminManagement")]
    public class PayrollsController : Controller
    {
        private readonly HrmanagementContext _context;
        private readonly IHttpClientFactory _httpClientFactory;

        public PayrollsController(HrmanagementContext context, IHttpClientFactory httpClientFactory)
        {
            _context = context;
            _httpClientFactory = httpClientFactory;
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
                            Status = "Chưa thanh toán",
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

        // GET: AdminManagement/Payrolls/PaymentMomo/5
        public async Task<IActionResult> PaymentMomo(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var payroll = await _context.Payrolls
                .FirstOrDefaultAsync(p => p.PayrollId == id);
            if (payroll == null)
            {
                return NotFound();
            }

            // Check if NetSalary is null, provide a default value or handle accordingly
            if (!payroll.NetSalary.HasValue)
            {
                TempData["Error"] = "Không thể thanh toán vì lương ròng không hợp lệ.";
                return RedirectToAction(nameof(Index));
            }

            var paymentUrl = await CreateMoMoPayment(payroll.PayrollId, payroll.NetSalary.Value);
            if (!string.IsNullOrEmpty(paymentUrl))
            {
                HttpContext.Session.SetInt32("PendingPayrollId", payroll.PayrollId);
                return Redirect(paymentUrl);
            }
            else
            {
                TempData["Error"] = "Không thể tạo yêu cầu thanh toán MoMo.";
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: AdminManagement/Payrolls/MoMoCallback
        [HttpGet]
        public async Task<IActionResult> MoMoCallback(string orderId, string resultCode, string message)
        {
            var payroll = await _context.Payrolls
                .FirstOrDefaultAsync(p => "MM" + p.PayrollId == orderId);
            if (payroll == null)
            {
                TempData["Error"] = "Không tìm thấy bản ghi lương.";
                return RedirectToAction(nameof(Index));
            }

            if (resultCode == "0")
            {
                payroll.Status = "Đã thanh toán";
                _context.Payrolls.Update(payroll);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Thanh toán MoMo thành công.";
            }
            else
            {
                TempData["Error"] = $"Thanh toán MoMo thất bại: {message}";
            }

            return RedirectToAction(nameof(Index));
        }

        // Hàm tạo yêu cầu thanh toán MoMo
        private async Task<string> CreateMoMoPayment(int payrollId, decimal amount)
        {
            string endpoint = "https://test-payment.momo.vn/v2/gateway/api/create";
            string partnerCode = "MOMO_ATM_DEV";
            string accessKey = "w9gEg8bjA2AM2Cvr";
            string secretKey = "mD9QAVi4cm9N844jh5Y2tqjWaaJoGVFM";
            string orderInfo = $"Thanh toán lương #{payrollId}";
            string redirectUrl = "https://localhost:7268/AdminManagement/Payrolls/MoMoCallback";
            string ipnUrl = "https://localhost:7268/AdminManagement/Payrolls/MoMoCallback";
            string requestId = Guid.NewGuid().ToString();
            string orderIdStr = $"MM{payrollId}";
            string amountStr = ((int)amount).ToString();

            var rawData = $"accessKey={accessKey}&amount={amountStr}&extraData=&ipnUrl={ipnUrl}&orderId={orderIdStr}&orderInfo={orderInfo}&partnerCode={partnerCode}&redirectUrl={redirectUrl}&requestId={requestId}&requestType=payWithATM";
            var signature = HmacSha256(secretKey, rawData);

            var requestData = new
            {
                partnerCode,
                partnerName = "HRMmanagement",
                storeId = "HRMStore",
                requestId,
                amount = amountStr,
                orderId = orderIdStr,
                orderInfo,
                redirectUrl,
                ipnUrl,
                lang = "vi",
                requestType = "payWithATM",
                autoCapture = true,
                extraData = "",
                signature
            };

            using var client = _httpClientFactory.CreateClient();
            var json = JsonConvert.SerializeObject(requestData);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PostAsync(endpoint, content);
            if (response.IsSuccessStatusCode)
            {
                var responseData = JsonConvert.DeserializeObject<dynamic>(await response.Content.ReadAsStringAsync());
                return responseData.payUrl;
            }

            return null;
        }

        // Hàm tạo chữ ký HMAC SHA256
        private string HmacSha256(string key, string data)
        {
            using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(key));
            var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(data));
            return BitConverter.ToString(hash).Replace("-", "").ToLower();
        }

        private bool PayrollExists(int id)
        {
            return _context.Payrolls.Any(e => e.PayrollId == id);
        }
    }
}