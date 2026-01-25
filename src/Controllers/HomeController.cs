using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SiteSB.Data;
using SiteSB.Models;
using SiteSB.ViewModels;
using System.Diagnostics;

namespace SiteSB.Controllers
{
    public class HomeController(AppDbContext context, ILogger<HomeController> logger) : Controller
    {
        private readonly AppDbContext _context = context;
        private readonly ILogger<HomeController> _logger = logger;

        public async Task<IActionResult> Index()
        {
            // Проверяем авторизацию
            var userIdStr = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userIdStr))
            {
                return RedirectToAction("Login", "Auth");
            }

            try
            {
                if (int.TryParse(userIdStr, out int userId))
                {
                    var user = await _context.Users
                        .Include(u => u.Role)
                        .FirstOrDefaultAsync(u => u.Id == userId);

                    if (user == null)
                    {
                        _logger.LogWarning("Пользователь с Id {UserId} не найден", userId);
                        HttpContext.Session.Clear();
                        return RedirectToAction("Login", "Auth");
                    }

                    ViewBag.FullName = user.FullName;
                    ViewBag.Role = user.Role?.Name ?? "Не указана";
                    ViewBag.LastLogin = user.LastLogin?.ToString("dd.MM.yyyy HH:mm") ?? "Никогда";
                }
                else
                {
                    _logger.LogWarning("Неверный формат UserId в сессии: {UserIdStr}", userIdStr);
                    return RedirectToAction("Login", "Auth");
                }

                // Создаем модель для дашборда
                var viewModel = new DashboardViewModel
                {
                    TotalDepositors = await _context.Depositors.CountAsync(),
                    ActiveDeposits = await _context.Deposits.CountAsync(d => d.Status == "Активный"),
                    TotalAmount = await _context.Deposits.Where(d => d.Status == "Активный").SumAsync(d => d.Amount),
                    RecentTransactions = await _context.Transactions
                        .OrderByDescending(t => t.TransactionDate)
                        .Take(10)
                        .ToListAsync()
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при загрузке главной страницы");
                ViewBag.Error = "Произошла ошибка при загрузке данных";
                return View(new DashboardViewModel());
            }
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
