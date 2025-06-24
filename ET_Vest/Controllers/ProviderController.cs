using ET_Vest.Data;
using ET_Vest.Data.ViewModels;
using ET_Vest.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Drawing.Drawing2D;

namespace ET_Vest.Controllers
{
    [Authorize(Roles = "Owner, Admin")]

    public class ProviderController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProviderController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var provider = _context.Providers
            .Include(p => p.PrintEditionProviders)
            .ThenInclude(p => p.PrintedEdition)
            .ToList();

            return View(provider);
        }

        public IActionResult Add()
        {
            ViewBag.PrintedEditions = _context.PrintedEditionProviders.Include
                (ma => ma.PrintedEdition).ToList();

            return View();
        }

        [HttpPost]
        public IActionResult Add(Provider provider)
        { 
            _context.Providers.Add(provider);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        public IActionResult Edit(int id)
        {
            var provider = _context.Providers.Find(id);

            if (provider == null)
            {
                return NotFound();
            }

            var printedEditionProvider = _context.PrintedEditionProviders
          .Include(pe => pe.PrintedEdition)
          .FirstOrDefault(pe => pe.ProviderId == id);

            var viewModel = new PrintedEditionProviderViewModel
            {
                Provider = provider,
                PrintedEditionProvider = printedEditionProvider
            };
            ViewBag.PrintedEditions = new SelectList(_context.PrintedEditions, "PrintedEditionId", "Title");
            return View(viewModel);
        }
        [HttpPost]
        public IActionResult Edit(PrintedEditionProviderViewModel viewModel)
        {
            //update provider details
            _context.Providers.Update(viewModel.Provider);
            _context.SaveChanges();
            return RedirectToAction("Index");

        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            var provider = _context.Providers.Find(id);

            if (provider == null)
            {
                return NotFound();
            }

            _context.Providers.Remove(provider);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}