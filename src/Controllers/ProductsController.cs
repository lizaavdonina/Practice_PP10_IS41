using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SiteSB.Data;
using SiteSB.Models;

namespace SiteSB.Controllers
{
    // Исправление IDE0290: используем первичный конструктор
    public class ProductsController(AppDbContext context) : Controller
    {
        private readonly AppDbContext _context = context;

        // GET: Products
        public async Task<IActionResult> Index()
        {
            if (HttpContext.Session.GetString("UserId") == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            var depositTypes = await _context.DepositTypes.ToListAsync();
            return View(depositTypes);
        }

        // GET: Products/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var depositType = await _context.DepositTypes
                .FirstOrDefaultAsync(m => m.Id == id);

            if (depositType == null)
            {
                return NotFound();
            }

            return View(depositType);
        }

        // GET: Products/Create
        public IActionResult Create()
        {
            if (HttpContext.Session.GetString("UserId") == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            return View();
        }

        // POST: Products/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,InterestRate,TermMonths,MinAmount,CanReplenish,CanWithdraw,Description")] DepositType depositType)
        {
            if (ModelState.IsValid)
            {
                _context.Add(depositType);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(depositType);
        }

        // GET: Products/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var depositType = await _context.DepositTypes.FindAsync(id);
            if (depositType == null)
            {
                return NotFound();
            }

            return View(depositType);
        }

        // POST: Products/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,InterestRate,TermMonths,MinAmount,CanReplenish,CanWithdraw,Description")] DepositType depositType)
        {
            if (id != depositType.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(depositType);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DepositTypeExists(depositType.Id))
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
            return View(depositType);
        }

        // GET: Products/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var depositType = await _context.DepositTypes
                .FirstOrDefaultAsync(m => m.Id == id);

            if (depositType == null)
            {
                return NotFound();
            }

            return View(depositType);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var depositType = await _context.DepositTypes.FindAsync(id);
            if (depositType != null)
            {
                _context.DepositTypes.Remove(depositType);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool DepositTypeExists(int id)
        {
            return _context.DepositTypes.Any(e => e.Id == id);
        }
    }
}
