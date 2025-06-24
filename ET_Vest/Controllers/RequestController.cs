using ET_Vest.Data;
using ET_Vest.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace ET_Vest.Controllers
{
    public class RequestController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;


        public RequestController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var requests = _context.Requests
           .Include(t => t.TradeObject)
           .Include(t => t.PrintedEdition)
           .Include(t => t.Provider)
           .ToList();

            if (User.IsInRole("Employee"))
            {
                var user = User.FindFirstValue(ClaimTypes.NameIdentifier);

                var empRequested = _context.Requests
                    .Where(m => m.TradeObject.EmployeeId == user);

                return View(empRequested);
            }
            else
            {
                return View(requests);
            }
        }

        public async Task<IActionResult> Add()
        {
            if (User.IsInRole("Employee"))
            {
                var user = User.FindFirstValue(ClaimTypes.NameIdentifier);

                ViewBag.TradeObjects = _context.TradeObjects
                    .Where(to => to.EmployeeId == user);
                ViewBag.PrintedEditions = _context.PrintedEditions.ToList();
                ViewBag.Providers = _context.Providers.ToList();

                return View();
            }
            else
            {
                ViewBag.TradeObjects = _context.TradeObjects.ToList();
                ViewBag.PrintedEditions = _context.PrintedEditions.ToList();
                ViewBag.Providers = _context.Providers.ToList();

                return View();
            }
        }
        [Authorize(Roles = "Owner, Employee")]
        [HttpPost]
        public IActionResult Add(Request request)
        {
            request.RequestDate = DateTime.Today;

            if (User.IsInRole("Owner"))
            {
                request.Status = RequestStatus.SentToProvider;
            }
            else
            {
                request.Status = RequestStatus.Pending;
            }

            _context.Requests.Add(request);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Edit(int id)
        {
            var request = _context.Requests
                .Include(m => m.TradeObject)
                .Include(m => m.PrintedEdition)
                .Include(m => m.Provider)
                .FirstOrDefault(m => m.Id == id);
            if (request == null)
            {
                return NotFound();
            }
            ViewBag.TradeObjects = _context.TradeObjects.ToList();
            ViewBag.PrintedEditions = _context.PrintedEditions.ToList();
            ViewBag.Providers = _context.Providers.ToList();

            return View(request);
        }

        [Authorize(Roles = "Owner")]
        [HttpPost]
        public IActionResult Edit(Request request)
        {
            request.RequestDate = DateTime.Today;

            request.Status = RequestStatus.SentToProvider;

            _context.Requests.Update(request);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            var request = _context.Requests.Find(id);

            if (request == null)
            {
                return NotFound();
            }

            _context.Requests.Remove(request);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult SentToOwner(int id)
        {
            var request = _context.Requests.Find(id);

            if (request == null)
            {
                return NotFound();
            }

            request.Status = RequestStatus.SentToOwner; 

            _context.SaveChanges();

            return RedirectToAction("Index");
        }
        [HttpPost]
        public IActionResult SentToProvider(int id)
        {
            var request = _context.Requests.Find(id);

            if (request == null)
            {
                return NotFound();
            }

            request.Status = RequestStatus.SentToProvider;

            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Rejected(int id)
        {
            var request = _context.Requests.Find(id);

            if (request == null)
            {
                return NotFound();
            }

            request.Status = RequestStatus.Rejected; 

            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult DoneRequest(int id)
        {
            var request = _context.Requests.Find(id);

            if (request == null)
            {
                return NotFound();
            }

            // Mark the request as done
            request.Status = RequestStatus.Completed;

            // Find or create an inventory entry for the trade object with the printed edition
            var inventoryEntry = _context.Inventories.FirstOrDefault(
                i => i.TradeObjectId == request.TradeObjectId &&
                     i.PrintedEditionId == request.PrintedEditionId);

            if (inventoryEntry == null)
            {
                // If inventory entry does not exist, create a new one
                inventoryEntry = new Inventory
                {
                    TradeObjectId = request.TradeObjectId,
                    PrintedEditionId = request.PrintedEditionId,
                    Quantity = request.RequestedQuantity // Set the quantity from the request
                };
                _context.Inventories.Add(inventoryEntry);
            }
            else
            {
                // If inventory entry exists, increase the quantity
                inventoryEntry.Quantity += request.RequestedQuantity;
            }

            _context.SaveChanges();

            return RedirectToAction("Index");
        }

    }
}
