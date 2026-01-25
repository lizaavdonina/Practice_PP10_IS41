using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SiteSB.Data;
using SiteSB.Helpers;
using SiteSB.Models;

namespace SiteSB.Controllers
{
    public class AuthController(AppDbContext context, ILogger<AuthController> logger) : Controller
    {
        private readonly AppDbContext _context = context;
        private readonly ILogger<AuthController> _logger = logger;

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string username, string password)
        {
            try
            {
                if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                {
                    ViewBag.Error = "Имя пользователя и пароль обязательны";
                    return View();
                }

                // Ищем пользователя по имени
                var user = await _context.Users
                    .Include(u => u.Role)
                    .FirstOrDefaultAsync(u => u.Username == username);

                if (user == null)
                {
                    _logger.LogWarning("Попытка входа с несуществующим пользователем: {Username}", username);
                    ViewBag.Error = "Неверное имя пользователя или пароль";
                    return View();
                }

                // ВРЕМЕННОЕ РЕШЕНИЕ: для тестирования используем фиксированный пароль
                // Пароль для всех пользователей: Sber123!
                if (password != "Sber123!")
                {
                    _logger.LogWarning("Неверный пароль для пользователя: {Username}", username);
                    ViewBag.Error = "Неверное имя пользователя или пароль";
                    return View();
                }

                // Проверяем активность пользователя
                if (!user.IsActive)
                {
                    ViewBag.Error = "Учетная запись неактивна";
                    return View();
                }

                // Успешный вход
                HttpContext.Session.SetString("UserId", user.Id.ToString());
                HttpContext.Session.SetString("Username", user.Username ?? string.Empty);
                HttpContext.Session.SetString("UserRole", user.Role?.Name ?? string.Empty);
                HttpContext.Session.SetString("FullName", user.FullName ?? string.Empty);

                // Обновляем время последнего входа
                user.LastLogin = DateTime.Now;
                await _context.SaveChangesAsync();

                _logger.LogInformation("Успешный вход пользователя: {Username}", username);

                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при входе пользователя {Username}", username);
                ViewBag.Error = "Произошла ошибка при входе. Попробуйте снова.";
                return View();
            }
        }

        [HttpGet]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }

        // Метод для создания тестового пользователя с правильным паролем
        [HttpGet]
        public async Task<IActionResult> CreateTestUser()
        {
            try
            {
                // Создаем тестового пользователя с паролем "Sber123!"
                var testUser = new User
                {
                    Username = "testuser",
                    // Пароль будет проверяться напрямую, без хэширования
                    PasswordHash = "Sber123!",
                    Email = "test@sberbank.ru",
                    FullName = "Тестовый пользователь",
                    RoleId = 2, // Предполагаем, что RoleId 2 = Operator
                    IsActive = true,
                    LastLogin = null,
                    CreatedDate = DateTime.Now
                };

                _context.Users.Add(testUser);
                await _context.SaveChangesAsync();

                return Content("Тестовый пользователь создан!<br>" +
                    "Логин: <b>testuser</b><br>" +
                    "Пароль: <b>Sber123!</b><br>" +
                    "Также можно использовать существующих пользователей с тем же паролем");
            }
            catch (Exception ex)
            {
                return Content($"Ошибка: {ex.Message}");
            }
        }

        // Метод для обновления паролей в базе данных
        [HttpGet]
        public async Task<IActionResult> ResetAllPasswords()
        {
            try
            {
                var users = await _context.Users.ToListAsync();
                foreach (var user in users)
                {
                    user.PasswordHash = "Sber123!"; // Устанавливаем простой пароль
                }
                await _context.SaveChangesAsync();

                return Content("Пароли всех пользователей сброшены на: <b>Sber123!</b><br>" +
                    "Теперь можно войти с любым пользователем из базы с этим паролем.");
            }
            catch (Exception ex)
            {
                return Content($"Ошибка: {ex.Message}");
            }
        }

        // Метод для инициализации базы данных
        [HttpGet]
        public async Task<IActionResult> InitializeDatabase()
        {
            try
            {
                // Проверяем существование таблицы Users
                var usersTableExists = await _context.Database.ExecuteSqlRawAsync(@"
                    IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Users')
                    BEGIN
                        CREATE TABLE [Users] (
                            [Id] INT IDENTITY(1,1) PRIMARY KEY,
                            [Username] NVARCHAR(100) NOT NULL UNIQUE,
                            [PasswordHash] NVARCHAR(MAX) NOT NULL,
                            [Email] NVARCHAR(200) NULL,
                            [FullName] NVARCHAR(200) NOT NULL,
                            [RoleId] INT NOT NULL DEFAULT 2,
                            [IsActive] BIT NOT NULL DEFAULT 1,
                            [LastLogin] DATETIME NULL,
                            [CreatedAt] DATETIME NOT NULL DEFAULT GETDATE()
                        );
                    END");

                // Проверяем существование таблицы Roles
                await _context.Database.ExecuteSqlRawAsync(@"
                    IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Roles')
                    BEGIN
                        CREATE TABLE [Roles] (
                            [Id] INT IDENTITY(1,1) PRIMARY KEY,
                            [Name] NVARCHAR(50) NOT NULL UNIQUE,
                            [Description] NVARCHAR(200) NULL
                        );
                    END");

                // Добавляем стандартные роли
                await _context.Database.ExecuteSqlRawAsync(@"
                    IF NOT EXISTS (SELECT * FROM [Roles] WHERE [Name] = 'Admin')
                    BEGIN
                        INSERT INTO [Roles] ([Name], [Description]) 
                        VALUES ('Admin', 'Администратор системы');
                    END
                    
                    IF NOT EXISTS (SELECT * FROM [Roles] WHERE [Name] = 'Operator')
                    BEGIN
                        INSERT INTO [Roles] ([Name], [Description]) 
                        VALUES ('Operator', 'Оператор');
                    END
                    
                    IF NOT EXISTS (SELECT * FROM [Roles] WHERE [Name] = 'Analyst')
                    BEGIN
                        INSERT INTO [Roles] ([Name], [Description]) 
                        VALUES ('Analyst', 'Аналитик');
                    END");

                // Добавляем тестовых пользователей
                await _context.Database.ExecuteSqlRawAsync(@"
                    IF NOT EXISTS (SELECT * FROM [Users] WHERE [Username] = 'admin')
                    BEGIN
                        INSERT INTO [Users] ([Username], [PasswordHash], [Email], [FullName], [RoleId], [IsActive]) 
                        VALUES ('admin', 'Sber123!', 'admin@sberbank.ru', 'Иванов Иван Иванович', 1, 1);
                    END
                    
                    IF NOT EXISTS (SELECT * FROM [Users] WHERE [Username] = 'operator1')
                    BEGIN
                        INSERT INTO [Users] ([Username], [PasswordHash], [Email], [FullName], [RoleId], [IsActive]) 
                        VALUES ('operator1', 'Sber123!', 'operator@sberbank.ru', 'Петрова Мария Сергеевна', 2, 1);
                    END
                    
                    IF NOT EXISTS (SELECT * FROM [Users] WHERE [Username] = 'analyst1')
                    BEGIN
                        INSERT INTO [Users] ([Username], [PasswordHash], [Email], [FullName], [RoleId], [IsActive]) 
                        VALUES ('analyst1', 'Sber123!', 'analyst@sberbank.ru', 'Сидоров Алексей Петрович', 3, 1);
                    END");

                return Content("База данных инициализирована!<br>" +
                    "Таблицы созданы (если их не было)<br>" +
                    "Тестовые пользователи добавлены<br>" +
                    "Пароль для всех: <b>Sber123!</b><br><br>" +
                    "<a href='/Auth/Login'>Перейти к входу</a>");
            }
            catch (Exception ex)
            {
                return Content($"Ошибка: {ex.Message}<br><br>" +
                    $"Подробности: {ex.InnerException?.Message}");
            }
        }
    }
}
