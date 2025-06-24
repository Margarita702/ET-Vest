using Azure.Core;
using ET_Vest.Data;
using ET_Vest.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Security.Claims;

namespace ET_Vest.Controllers
{
    public class InventoryController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public InventoryController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var inventory = _context.Inventories
                .Include(t => t.TradeObject)
                .Include(t => t.PrintedEdition)
                .ToList();

            if (User.IsInRole("Employee"))
            {
                // Get the current logged-in user's ID
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                // Query the inventories where the trade object's EmployeeId matches the logged-in user's ID
                var inventories = await _context.Inventories
                    .Include(i => i.TradeObject)
                    .Where(i => i.TradeObject.EmployeeId == userId)
                    .ToListAsync();

                return View(inventories);
            }
            else
            {
                return View(inventory);
            }
        }

        public async Task<IActionResult> Add()
        {
            if (User.IsInRole("Employee"))
            {
                var user = User.FindFirstValue(ClaimTypes.NameIdentifier);

                // You can pass the trade object directly to the view
                ViewBag.TradeObjects = _context.TradeObjects.Where(to => to.EmployeeId == user);
                ViewBag.PrintedEditions = _context.PrintedEditions.ToList();

                return View();
            }
            else
            {
                ViewBag.TradeObjects = _context.TradeObjects.ToList();
                ViewBag.PrintedEditions = _context.PrintedEditions.ToList();

                return View();
            }
        }

        [HttpPost]
        public IActionResult Add(Inventory inventory)
        {
            // Check if the same TradeObject with its PrintedEdition already exists
            var existingInventory = _context.Inventories.FirstOrDefault(
                i => i.TradeObjectId == inventory.TradeObjectId && i.PrintedEditionId == inventory.PrintedEditionId);

            if (existingInventory != null)
            {
                // If it exists, just update the quantity
                existingInventory.Quantity += inventory.Quantity;
            }
            else
            {
                // If it doesn't exist, add a new inventory entry
                _context.Inventories.Add(inventory);
            }

            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Edit(int id)
        {

            if (User.IsInRole("Employee"))
            {
                var user = User.FindFirstValue(ClaimTypes.NameIdentifier);

                var empInventory = _context.Inventories
                    .Where(m => m.TradeObject.EmployeeId == user)
                    .Include(e => e.PrintedEdition)
                     .FirstOrDefault(m => m.Id == id); ;

                ViewBag.TradeObjects = _context.TradeObjects.Where(to => to.EmployeeId == user);
                ViewBag.PrintedEditions = _context.PrintedEditions.ToList();

                return View(empInventory);
            }
            else
            {
                var inventory = _context.Inventories
              .Include(m => m.TradeObject)
              .Include(m => m.PrintedEdition)
              .FirstOrDefault(m => m.Id == id);

                ViewBag.TradeObjects = _context.TradeObjects.ToList();
                ViewBag.PrintedEditions = _context.PrintedEditions.ToList();

                return View(inventory);
            }
        }

        [HttpPost]
        public IActionResult Edit(Inventory inventory)
        {
            _context.Inventories.Update(inventory);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        [Authorize(Roles = "Owner")]
        [HttpPost]
        public IActionResult Delete(int id)
        {
            var inventory = _context.Inventories.Find(id);

            if (inventory == null)
            {
                return NotFound();
            }

            _context.Inventories.Remove(inventory);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Dispose(int id)
        {
            var inventoryItem = _context.Inventories.Include(i => i.PrintedEdition).FirstOrDefault(i => i.Id == id);

            if (inventoryItem == null)
            {
                return NotFound();
            }

            if (inventoryItem.PrintedEdition.Category == Models.Enums.Category.Newspaper || inventoryItem.PrintedEdition.Category == Models.Enums.Category.Magazine)
            {
                inventoryItem.IsDisposed = true;
                _context.SaveChanges();
            }

            return RedirectToAction("Index");
        }


    }
}
