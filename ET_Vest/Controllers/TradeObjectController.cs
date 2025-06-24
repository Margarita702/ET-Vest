


using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ET_Vest.Models;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using ET_Vest.Data;
using Microsoft.AspNetCore.Authorization;

namespace ET_Vest.Controllers
{
    [Authorize(Roles = "Owner, Admin")]
    public class TradeObjectController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;

        public TradeObjectController(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var tradeObjects = await _context.TradeObjects.Include(t => t.Employee).ToListAsync();
            return View(tradeObjects);
        }

        public async Task<IActionResult> Add()
        {
            var employees = await _userManager.GetUsersInRoleAsync("Employee");

            ViewBag.Employees = new SelectList(employees, "Id", "Name");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(TradeObject tradeObject)
        {
            ModelState.Remove("Employee");
            ModelState.Remove("Requests");
            ModelState.Remove("Sales");

            if (ModelState.IsValid)
            {
                _context.Add(tradeObject);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            var employees = await _userManager.GetUsersInRoleAsync("Employee");
            ViewBag.Employees = new SelectList(employees, "Id", "Name", tradeObject.EmployeeId);

            return View("Add", tradeObject);

        }

        // GET: TradeObject/Edit/{id}
        public async Task<IActionResult> Edit(int id)
        {
            var tradeObject = await _context.TradeObjects.FindAsync(id);

            if (tradeObject == null)
            {
                return NotFound();
            }

            var employees = await _userManager.GetUsersInRoleAsync("Employee");
            ViewBag.Employees = new SelectList(employees, "Id", "Name", tradeObject.EmployeeId);

            return View(tradeObject);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, TradeObject tradeObject)
        {
            ModelState.Remove("Employee");
            ModelState.Remove("Requests");
            ModelState.Remove("Sales");
            if (ModelState.IsValid)
            {
                if (id != tradeObject.Id)
                {
                    return NotFound();
                }
                _context.Update(tradeObject);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            var employees = await _userManager.GetUsersInRoleAsync("Employee");
            ViewBag.Employees = new SelectList(employees, "Id", "Name", tradeObject.EmployeeId);

            return View("Edit", tradeObject);

        }

        public async Task<IActionResult> Delete(int id)
        {
            var tradeObject = await _context.TradeObjects
                                            .Include(t => t.Employee)
                                            .FirstOrDefaultAsync(m => m.Id == id);

            if (tradeObject == null)
            {
                return NotFound();
            }

            return View(tradeObject);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var tradeObject = await _context.TradeObjects.FindAsync(id);

            if (tradeObject == null)
            {
                return NotFound();
            }

            _context.TradeObjects.Remove(tradeObject);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


    }
}
