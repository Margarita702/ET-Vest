using Azure.Core;
using ET_Vest.Data;
using ET_Vest.Data.ViewModels;
using ET_Vest.Models;
using ET_Vest.Models.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Net.Sockets;
using System.Reflection;
using System.Security.Claims;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace ET_Vest.Controllers
{
    public class SaleController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public SaleController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var sales = _context.Sales
             .Include(t => t.TradeObject)
             .Include(t => t.PrintedEdition)
             .ToList();

            if (User.IsInRole("Employee"))
            {
                var user = User.FindFirstValue(ClaimTypes.NameIdentifier);

                var empSales = _context.Sales.Where(m => m.TradeObject.EmployeeId == user)
                .Where(s => s.DateOfSale == DateTime.Today)
                .ToList();

                var totalSum = CalculateTotalSum(empSales);
                ViewBag.TotalSum = totalSum;

                return View(empSales);
            }
            else
            {
                var totalSum = CalculateTotalSum(sales);
                ViewBag.TotalSum = totalSum;

                return View(sales);
            }
        }

        public async Task<IActionResult> Add()
        {
            var user = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // You can pass the trade object directly to the view
            ViewBag.TradeObjects = _context.TradeObjects.Where(to => to.EmployeeId == user);
            ViewBag.PrintedEditions = _context.PrintedEditions.ToList();

            return View();
        }

        [Authorize(Roles = "Employee")]
        [HttpPost]
        public IActionResult Add(Sale sale)
        {
            sale.DateOfSale = DateTime.Today;
            // Check if a trade object with a printed edition and quantity <= inventory exists
            var inventoryEntry = _context.Inventories.FirstOrDefault(
                i => i.TradeObjectId == sale.TradeObjectId &&
                     i.PrintedEditionId == sale.PrintedEditionId &&
                     i.Quantity >= sale.SoldQuantity);

            if (inventoryEntry == null || inventoryEntry.Quantity < sale.SoldQuantity)
            {
                // If inventory does not exist or quantity is insufficient, return error message
                ModelState.AddModelError(string.Empty, $"Тази продажба не може да бъде извършена. Няма достатъчно печатни издания в този обект!");

                // Retrieve the lists needed for the view
                ViewBag.PrintedEditions = _context.PrintedEditions.ToList();
                ViewBag.TradeObjects = _context.TradeObjects.ToList();

                // Return to the Add view with error message and populated lists
                return View(sale);
            }

            if (inventoryEntry != null && inventoryEntry.IsDisposed == true)
            {
                // Return error message indicating that the sale cannot be made for defective items
                ModelState.AddModelError(string.Empty, $"Продажба на бракувано издание не е възможна.");

                // Retrieve the lists needed for the view
                ViewBag.PrintedEditions = _context.PrintedEditions.ToList();
                ViewBag.TradeObjects = _context.TradeObjects.ToList();

                // Return to the Add view with error message and populated lists
                return View(sale);
            }

            inventoryEntry.Quantity -= sale.SoldQuantity;

                _context.Sales.Add(sale);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
     

        }

        [Authorize(Roles = "Employee")]
        [HttpPost]
        public IActionResult Delete(int id)
        {
            var sale = _context.Sales
                            .Include(s => s.PrintedEdition)
                            .Include(s => s.TradeObject)
                            .FirstOrDefault(s => s.SalesId == id);

            if (sale == null)
            {
                return NotFound();
            }

            // Increase the quantity in the inventory
            var inventoryEntry = _context.Inventories.FirstOrDefault(
                i => i.TradeObjectId == sale.TradeObjectId &&
                     i.PrintedEditionId == sale.PrintedEditionId);

            if (inventoryEntry != null)
            {
                inventoryEntry.Quantity += sale.SoldQuantity;
            }
            else
            {
                // Create a new inventory entry if it doesn't exist
                _context.Inventories.Add(new Inventory
                {
                    TradeObjectId = sale.TradeObjectId,
                    PrintedEditionId = sale.PrintedEditionId,
                    Quantity = sale.SoldQuantity
                });
            }

            // Remove the sale entry
            _context.Sales.Remove(sale);
            // Save changes to the database
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        private decimal CalculateTotalSum(List<Sale> filteredSales)
        {
            return filteredSales.Sum(s => s.Total);
        }

        [Authorize(Roles = "Owner, Admin")]
        public IActionResult SalesByDateAndPE(DateTime? startDate, DateTime? endDate, string tradeObjectSearch, string printedEditionSearch)
        {
            // Set default values if no dates are provided
            startDate ??= DateTime.MinValue;
            endDate ??= DateTime.MaxValue;

            // Retrieve sales for the specified period, trade object, and search term
            var salesQuery = _context.Sales
                .Include(s => s.PrintedEdition)
                .Include(t => t.TradeObject)
                .Where(s => s.DateOfSale >= startDate && s.DateOfSale <= endDate);

            // Apply search filter if a search term is provided for PrintedEditions
            if (!string.IsNullOrWhiteSpace(printedEditionSearch))
            {
                salesQuery = salesQuery.Where(s => s.PrintedEdition.Title.Contains(printedEditionSearch));
            }

            // Apply search filter if a search term is provided for TradeObjects
            if (!string.IsNullOrWhiteSpace(tradeObjectSearch))
            {
                salesQuery = salesQuery.Where(s => s.TradeObject.Name.Contains(tradeObjectSearch));
            }

            // Order by date and convert to a list
            var filteredSales = salesQuery.OrderBy(s => s.DateOfSale).ToList();

            // Calculate total sum based on the filtered sales
            var totalSum = CalculateTotalSum(filteredSales);

            // Pass the list of sales, selected period, trade object ID, and total sum to the Index view
            ViewBag.StartDate = startDate;
            ViewBag.EndDate = endDate;
            ViewBag.TradeObjectSearch = tradeObjectSearch;
            ViewBag.PrintedEditionSearch = printedEditionSearch;
            ViewBag.TotalSum = totalSum;

            return View("Index", filteredSales);
        }


    }

}