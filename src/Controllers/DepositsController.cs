using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SiteSB.Data;
using SiteSB.Models;

namespace SiteSB.Controllers
{
    // Исправление IDE0290: используем первичный конструктор
    public class DepositsController(AppDbContext context) : Controller
    {
        private readonly AppDbContext _context = context;

        // GET: Deposits
        public async Task<IActionResult> Index()
        {
            if (HttpContext.Session.GetString("UserId") == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            var deposits = await _context.Deposits
                .Include(d => d.Depositor)
                .Include(d => d.DepositType)
                .ToListAsync();
            return View(deposits);
        }

        // GET: Deposits/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var deposit = await _context.Deposits
                .Include(d => d.Depositor)
                .Include(d => d.DepositType)
                .Include(d => d.Transactions)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (deposit == null)
            {
                return NotFound();
            }

            return View(deposit);
        }

        // GET: Deposits/Create
        public IActionResult Create()
        {
            if (HttpContext.Session.GetString("UserId") == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            ViewBag.Depositors = _context.Depositors.ToList();
            ViewBag.DepositTypes = _context.DepositTypes.ToList();
            return View();
        }

        // POST: Deposits/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ContractNumber,DepositorId,DepositTypeId,Amount,OpenDate,Status")] Deposit deposit)
        {
            if (ModelState.IsValid)
            {
                deposit.CreatedDate = DateTime.Now;
                deposit.AccruedInterest = 0;

                deposit.Status ??= "Активный";

                _context.Add(deposit);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Depositors = _context.Depositors.ToList();
            ViewBag.DepositTypes = _context.DepositTypes.ToList();
            return View(deposit);
        }

        // GET: Deposits/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var deposit = await _context.Deposits.FindAsync(id);
            if (deposit == null)
            {
                return NotFound();
            }

            ViewBag.Depositors = _context.Depositors.ToList();
            ViewBag.DepositTypes = _context.DepositTypes.ToList();
            return View(deposit);
        }

        // POST: Deposits/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ContractNumber,DepositorId,DepositTypeId,Amount,OpenDate,CloseDate,Status,AccruedInterest,CreatedDate")] Deposit deposit)
        {
            if (id != deposit.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(deposit);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DepositExists(deposit.Id))
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

            ViewBag.Depositors = _context.Depositors.ToList();
            ViewBag.DepositTypes = _context.DepositTypes.ToList();
            return View(deposit);
        }

        // GET: Deposits/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var deposit = await _context.Deposits
                .Include(d => d.Depositor)
                .Include(d => d.DepositType)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (deposit == null)
            {
                return NotFound();
            }

            return View(deposit);
        }

        // POST: Deposits/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var deposit = await _context.Deposits.FindAsync(id);
            if (deposit != null)
            {
                _context.Deposits.Remove(deposit);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        // GET: Deposits/Close/5
        public async Task<IActionResult> Close(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var deposit = await _context.Deposits
                .Include(d => d.Depositor)
                .Include(d => d.DepositType)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (deposit == null)
            {
                return NotFound();
            }

            return View(deposit);
        }

        // POST: Deposits/Close/5
        [HttpPost, ActionName("Close")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CloseConfirmed(int id)
        {
            var deposit = await _context.Deposits.FindAsync(id);
            if (deposit != null)
            {
                // Исправление IDE0074: используем составное присваивание
                deposit.Status = "Закрыт";
                deposit.CloseDate = DateTime.Now;
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool DepositExists(int id)
        {
            return _context.Deposits.Any(e => e.Id == id);
        }
    }
}
