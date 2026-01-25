using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SiteSB.Data;
using SiteSB.Models;
using System.Linq;
using System.Threading.Tasks;

namespace SiteSB.Controllers
{
    public class TransactionsController : Controller
    {
        private readonly AppDbContext _context;

        public TransactionsController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                // Проверяем авторизацию
                var userIdStr = HttpContext.Session.GetString("UserId");
                if (string.IsNullOrEmpty(userIdStr))
                {
                    return RedirectToAction("Login", "Auth");
                }

                // Получаем транзакции (без User, так как его нет в таблице)
                var transactions = await _context.Transactions
                    .Include(t => t.Deposit) // Включаем связанный вклад
                    .OrderByDescending(t => t.TransactionDate)
                    .Take(50)
                    .ToListAsync();

                return View(transactions);
            }
            catch (Exception ex)
            {
                // Логируем ошибку
                Console.WriteLine($"Ошибка при получении транзакций: {ex.Message}");

                // Возвращаем пустой список или перенаправляем на главную
                return View(new List<Transaction>());
            }
        }
    }
}
