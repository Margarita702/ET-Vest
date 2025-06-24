using ET_Vest.Data;
using ET_Vest.Data.ViewModels;
using ET_Vest.Models;
using ET_Vest.Models.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.DotNet.Scaffolding.Shared;
using Microsoft.EntityFrameworkCore;

namespace ET_Vest.Controllers
{
    [Authorize(Roles = "Owner, Admin")]

    public class PrintedEditionController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PrintedEditionController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var printedEdition = _context.PrintedEditions
            .Include(m => m.PrintEditionProviders)
            .ToList();

            return View(printedEdition);
        }

        //Add PrintedEdition
        public IActionResult Add()
        {
            ViewBag.Providers = _context.PrintedEditionProviders.Include
                (pp => pp.Provider).ToList();

            return View();
        }

        [HttpPost]
        public IActionResult Add(PrintedEdition printedEdition)
        {
            ModelState.Remove("PrintEditionProviders");
            ModelState.Remove("Requests");
            ModelState.Remove("Sale");

            if (ModelState.IsValid)
            {
                _context.PrintedEditions.Add(printedEdition);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }

            return View("Add", printedEdition);
        }

        //Update Printed Editions
        public IActionResult Edit(int id)
        {
            var printedEdition = _context.PrintedEditions.Find(id);

            if (printedEdition == null)
            {
                return NotFound();
            }

            var printedEditionProvider = _context.PrintedEditionProviders
                    .Include(pe => pe.Provider)
                    .FirstOrDefault(pe => pe.PrintedEditionId == id);

            var viewModel = new PrintedEditionProviderViewModel
            {
                PrintedEdition = printedEdition,
                PrintedEditionProvider = printedEditionProvider
            };
            ViewBag.Providers = new SelectList(_context.Providers, "ProviderId", "Name");
            return View(viewModel);
        }

        [HttpPost]
        public IActionResult Edit(PrintedEditionProviderViewModel viewModel)
        {
            //update edition details
            _context.PrintedEditions.Update(viewModel.PrintedEdition);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            var printedEditions = _context.PrintedEditions.Find(id);

            if (printedEditions == null)
            {
                return NotFound();
            }


            _context.PrintedEditions.Remove(printedEditions);
            _context.SaveChanges();
            return RedirectToAction("Index");


        }
    }
}

