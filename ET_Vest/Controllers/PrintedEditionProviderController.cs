using ET_Vest.Data;
using ET_Vest.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ET_Vest.Controllers
{
    [Authorize(Roles = "Owner, Admin")]

    public class PrintedEditionProviderController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PrintedEditionProviderController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var printedEditionProvider = _context.PrintedEditionProviders
            .Include(m => m.PrintedEdition)
            .Include(m => m.Provider)
            .ToList();

            return View(printedEditionProvider);
        }

        //Add PrintedEditionProvider
        public IActionResult Add()
        {
            ViewBag.PrintedEditions = _context.PrintedEditions.ToList();
            ViewBag.Providers = _context.Providers.ToList();

            return View();
        }

        [HttpPost]
        public IActionResult Add(PrintedEditionProvider printedEditionProvider)
        {
            _context.PrintedEditionProviders.Add(printedEditionProvider);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}