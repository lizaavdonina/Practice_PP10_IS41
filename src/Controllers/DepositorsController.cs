using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SiteSB.Data;
using SiteSB.Models;

namespace SiteSB.Controllers
{
    // Исправление IDE0100: используем первичный конструктор
    public class DepositorsController(AppDbContext context) : Controller
    {
        private readonly AppDbContext _context = context;

        // GET: Depositors
        public async Task<IActionResult> Index()
        {
            if (HttpContext.Session.GetString("UserId") == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            var depositors = await _context.Depositors
                .Include(d => d.ClientCategory)
                .ToListAsync();
            return View(depositors);
        }

        // GET: Depositors/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Исправление ошибки ThenInclude: изменили типы Include/ThenInclude
            var depositor = await _context.Depositors
                .Include(d => d.ClientCategory)
                .Include(d => d.Deposits!)
                    .ThenInclude(dep => dep.DepositType)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (depositor == null)
            {
                return NotFound();
            }

            return View(depositor);
        }

        // GET: Depositors/Create
        public IActionResult Create()
        {
            if (HttpContext.Session.GetString("UserId") == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            ViewBag.Categories = _context.ClientCategories.ToList();
            return View();
        }

        // POST: Depositors/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("INN,FullName,BirthDate,Phone,Email,Address,Passport,ClientCategoryId")] Depositor depositor)
        {
            if (ModelState.IsValid)
            {
                depositor.RegistrationDate = DateTime.Now;
                _context.Add(depositor);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Categories = _context.ClientCategories.ToList();
            return View(depositor);
        }

        // GET: Depositors/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var depositor = await _context.Depositors.FindAsync(id);
            if (depositor == null)
            {
                return NotFound();
            }
            ViewBag.Categories = _context.ClientCategories.ToList();
            return View(depositor);
        }

        // POST: Depositors/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,INN,FullName,BirthDate,Phone,Email,Address,Passport,ClientCategoryId,RegistrationDate")] Depositor depositor)
        {
            if (id != depositor.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(depositor);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DepositorExists(depositor.Id))
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
            ViewBag.Categories = _context.ClientCategories.ToList();
            return View(depositor);
        }

        // GET: Depositors/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var depositor = await _context.Depositors
                .Include(d => d.ClientCategory)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (depositor == null)
            {
                return NotFound();
            }

            return View(depositor);
        }

        // POST: Depositors/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var depositor = await _context.Depositors.FindAsync(id);
            if (depositor != null)
            {
                _context.Depositors.Remove(depositor);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool DepositorExists(int id)
        {
            return _context.Depositors.Any(e => e.Id == id);
        }
    }
}
