using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using System.Text;

namespace SiteSB.Controllers
{
    public class HashController : Controller
    {
        public IActionResult Generate(string password = "admin123")
        {
            var hash = HashPassword(password);
            ViewBag.Password = password;
            ViewBag.Hash = hash;
            return View();
        }

        private static string HashPassword(string password)
        {
            var bytes = Encoding.UTF8.GetBytes(password);
            var hash = SHA256.HashData(bytes);
            return Convert.ToBase64String(hash);
        }
    }
}
