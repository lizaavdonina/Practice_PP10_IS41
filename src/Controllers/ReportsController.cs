using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SiteSB.Data;
using SiteSB.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SiteSB.Controllers
{
    public class ReportsController(AppDbContext context) : Controller
    {
        private readonly AppDbContext _context = context;

        // Проверка авторизации для всех действий
        private bool CheckAuthorization()
        {
            return !string.IsNullOrEmpty(HttpContext.Session.GetString("UserId"));
        }

        // Отчёт по клиентам
        public async Task<IActionResult> Clients()
        {
            if (!CheckAuthorization())
                return RedirectToAction("Login", "Auth");

            try
            {
                var clients = await _context.Depositors
                    .Include(d => d.ClientCategory)
                    .OrderByDescending(d => d.RegistrationDate)
                    .ToListAsync();

                ViewBag.TotalClients = clients.Count;
                ViewBag.NewThisMonth = clients.Count(c => c.RegistrationDate.Month == DateTime.Now.Month);

                return View(clients);
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Ошибка при загрузке данных: {ex.Message}";
                return View(Array.Empty<Depositor>());
            }
        }

        // Финансовый отчёт
        public async Task<IActionResult> Financial()
        {
            if (!CheckAuthorization())
                return RedirectToAction("Login", "Auth");

            try
            {
                var financialData = new
                {
                    TotalDeposits = await _context.Deposits.SumAsync(d => d.Amount),
                    AverageDeposit = await _context.Deposits.AverageAsync(d => d.Amount),
                    ActiveDepositsCount = await _context.Deposits.CountAsync(d => d.Status == "Активный"),
                    TotalInterest = await _context.Deposits.SumAsync(d => d.AccruedInterest)
                };

                return View(financialData);
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Ошибка при загрузке данных: {ex.Message}";
                return View(new
                {
                    TotalDeposits = 0m,
                    AverageDeposit = 0m,
                    ActiveDepositsCount = 0,
                    TotalInterest = 0m
                });
            }
        }

        // Экспорт отчёта по клиентам в Excel
        public async Task<IActionResult> ExportClients()
        {
            if (!CheckAuthorization())
                return RedirectToAction("Login", "Auth");

            try
            {
                var clients = await _context.Depositors.ToListAsync();
                // Здесь будет логика экспорта в Excel
                return Content("Экспорт в Excel (в разработке)");
            }
            catch (Exception ex)
            {
                return Content($"Ошибка при экспорте: {ex.Message}");
            }
        }
    }
}
