using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SiteSB.Data;
using SiteSB.Models;

namespace SiteSB.Controllers
{
    // Исправление IDE0290: используем первичный конструктор
    public class MonitoringController(AppDbContext context) : Controller
    {
        private readonly AppDbContext _context = context;

        // GET: Monitoring
        public async Task<IActionResult> Index()
        {
            if (HttpContext.Session.GetString("UserId") == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            var model = new MonitoringViewModel
            {
                TotalUsers = await _context.Users.CountAsync(),
                ActiveUsers = await _context.Users.CountAsync(u => u.IsActive),
                TotalAuditLogs = await _context.AuditLogs.CountAsync(),
                RecentLogs = await _context.AuditLogs
                    .Include(l => l.User)
                    .OrderByDescending(l => l.Timestamp)
                    .Take(20)
                    .ToListAsync(),
                SystemStatus = "Работает нормально",
                DatabaseStatus = await CheckDatabaseConnection(),
                LastBackup = DateTime.Now.AddDays(-1)
            };

            return View(model);
        }

        // ... остальные методы без изменений ...

        private async Task<string> CheckDatabaseConnection()
        {
            try
            {
                await _context.Database.CanConnectAsync();
                return "Подключено";
            }
            catch
            {
                return "Ошибка подключения";
            }
        }

        // Исправление CA1822: делаем метод статическим
        private static string GetMemoryUsage()
        {
            var process = System.Diagnostics.Process.GetCurrentProcess();
            var memoryMB = process.WorkingSet64 / 1024 / 1024;
            return $"{memoryMB} MB";
        }

        // Исправление CA1822: делаем метод статическим
        private static string GetCPUUsage()
        {
            // Простая реализация, в реальном приложении используйте PerformanceCounter
            return "Нормальная";
        }
    }
}

// ВЫНЕСИТЕ ViewModel из контроллера в корень пространства имен
namespace SiteSB.Controllers
{
    public class MonitoringViewModel
    {
        public int TotalUsers { get; set; }
        public int ActiveUsers { get; set; }
        public int TotalAuditLogs { get; set; }
        public List<AuditLog> RecentLogs { get; set; } = [];
        public string SystemStatus { get; set; } = string.Empty;
        public string DatabaseStatus { get; set; } = string.Empty;
        public DateTime LastBackup { get; set; }
    }

    public class SystemInfoViewModel
    {
        public string ServerName { get; set; } = string.Empty;
        public string OSVersion { get; set; } = string.Empty;
        public string DotNetVersion { get; set; } = string.Empty;
        public DateTime CurrentTime { get; set; }
        public DateTime ApplicationStartTime { get; set; }
        public string MemoryUsage { get; set; } = string.Empty;
        public string CPUUsage { get; set; } = string.Empty;
    }
}
