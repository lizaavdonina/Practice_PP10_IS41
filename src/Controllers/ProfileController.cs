using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SiteSB.Data;
using SiteSB.Models;
using SiteSB.Helpers;

namespace SiteSB.Controllers
{
    // Исправление IDE0290: используем первичный конструктор
    public class ProfileController(AppDbContext context) : Controller
    {
        private readonly AppDbContext _context = context;

        // GET: Profile
        public async Task<IActionResult> Index()
        {
            if (HttpContext.Session.GetString("UserId") == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            var userId = int.Parse(HttpContext.Session.GetString("UserId")!);
            var user = await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
            {
                return RedirectToAction("Logout", "Auth");
            }

            return View(user);
        }

        // GET: Profile/Edit
        public async Task<IActionResult> Edit()
        {
            if (HttpContext.Session.GetString("UserId") == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            var userId = int.Parse(HttpContext.Session.GetString("UserId")!);
            var user = await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
            {
                return RedirectToAction("Logout", "Auth");
            }

            return View(user);
        }

        // POST: Profile/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([Bind("Id,Username,Email,FullName")] User userUpdate)
        {
            if (HttpContext.Session.GetString("UserId") == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            var userId = int.Parse(HttpContext.Session.GetString("UserId")!);
            var user = await _context.Users.FindAsync(userId);

            if (user == null)
            {
                return RedirectToAction("Logout", "Auth");
            }

            if (ModelState.IsValid)
            {
                // Обновляем только разрешенные поля
                user.Username = userUpdate.Username;
                user.Email = userUpdate.Email;
                user.FullName = userUpdate.FullName;

                try
                {
                    _context.Update(user);
                    await _context.SaveChangesAsync();

                    // Исправление CS8604: добавляем проверки на null перед установкой в сессию
                    HttpContext.Session.SetString("Username", user.Username ?? string.Empty);
                    HttpContext.Session.SetString("FullName", user.FullName ?? string.Empty);

                    ViewBag.SuccessMessage = "Профиль успешно обновлен";
                }
                catch (DbUpdateConcurrencyException)
                {
                    throw;
                }
            }

            return View(user);
        }

        // GET: Profile/ChangePassword
        public IActionResult ChangePassword()
        {
            if (HttpContext.Session.GetString("UserId") == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            return View();
        }

        // POST: Profile/ChangePassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(string currentPassword, string newPassword, string confirmPassword)
        {
            if (HttpContext.Session.GetString("UserId") == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            if (newPassword != confirmPassword)
            {
                ModelState.AddModelError("confirmPassword", "Пароли не совпадают");
                return View();
            }

            var userId = int.Parse(HttpContext.Session.GetString("UserId")!);
            var user = await _context.Users.FindAsync(userId);

            if (user == null)
            {
                return RedirectToAction("Logout", "Auth");
            }

            // Исправление CS8604: добавляем проверку PasswordHash на null
            if (!string.IsNullOrEmpty(user.PasswordHash) &&
                AuthHelper.VerifyPassword(currentPassword, user.PasswordHash))
            {
                // Устанавливаем новый пароль
                user.PasswordHash = AuthHelper.HashPassword(newPassword);
                await _context.SaveChangesAsync();

                ViewBag.SuccessMessage = "Пароль успешно изменен";
                return View();
            }
            else
            {
                ModelState.AddModelError("currentPassword", "Неверный текущий пароль");
                return View();
            }
        }

        // GET: Profile/Activity
        public async Task<IActionResult> Activity()
        {
            if (HttpContext.Session.GetString("UserId") == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            var userId = int.Parse(HttpContext.Session.GetString("UserId")!);

            var activities = await _context.AuditLogs
                .Where(a => a.UserId == userId)
                .OrderByDescending(a => a.Timestamp)
                .Take(50)
                .ToListAsync();

            return View(activities);
        }
    }
}
