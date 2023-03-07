﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EmmasEngines.Data;
using EmmasEngines.Models;
using EmmasEngines.Utilities;

namespace EmmasEngines.Controllers
{
    public class SuppliersController : Controller
    {
        private readonly EmmasEnginesContext _context;

        public SuppliersController(EmmasEnginesContext context)
        {
            _context = context;
        }

        // GET: Suppliers
        public async Task<IActionResult> Index(string SearchString, string actionButton, int? page, int? pageSizeID, string sortDirection = "asc",  string sortField = "Name")
        {
            //List of sort options
            string[] sortOptions = new[] { "ID", "FName" }; 
            
            var emmasEnginesContext = _context.Suppliers
                .Include(s => s.City)
                .AsNoTracking();

            if (!String.IsNullOrEmpty(SearchString))
            {
                emmasEnginesContext = emmasEnginesContext.Where(s => s.Name.ToLower().Contains(SearchString.ToLower())
                || s.Phone.Contains(SearchString)
                || s.Email.ToLower().Contains(SearchString.ToLower()));
                ViewData["Search Suppliers"] = SearchString;
            }

            //Sorting
            if(!String.IsNullOrEmpty(actionButton)) //Form submitted
            {
                page = 1; // Reset to page 1
                if(sortOptions.Contains(actionButton)) //Change of sort is requested
                {
                    if(actionButton == sortField) //Reverse order on same field
                    {
                        sortDirection = sortDirection == "asc" ? "desc" : "asc";
                    }
                    sortField = actionButton; //Sort by the button clicked
                }
            }

            //Which field and direction to sort by
            if (sortField == "Name")
            {
                if (sortDirection == "asc")
                {
                    emmasEnginesContext = emmasEnginesContext
                        .OrderBy(Suppliers => Suppliers.Name);
                }
                else
                {
                    emmasEnginesContext = emmasEnginesContext
                        .OrderByDescending(Suppliers => Suppliers.Name);
                }
            }
            else
            {
                if(sortDirection == "asc")
                {
                    emmasEnginesContext = emmasEnginesContext
                        .OrderBy(Supplier => Supplier.ID);
                }
                else
                {
                    emmasEnginesContext = emmasEnginesContext
                        .OrderByDescending(Supplier => Supplier.ID);
                }
            }
            //Set Sort for next time
            ViewData["sortField"] = sortField;
            ViewData["sortDirection"] = sortDirection;


            int pageSize = PageSizeHelper.SetPageSize(HttpContext, pageSizeID, "Supplier");
            ViewData["pageSizeID"] = PageSizeHelper.PageSizeList(pageSize);

            var pagedData = await PaginatedList<Supplier>.CreateAsync(emmasEnginesContext.AsNoTracking(), page ?? 1, pageSize);
            //return View(await emmasEnginesContext.ToListAsync());
            return View(pagedData);
        }

        // GET: Suppliers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Suppliers == null)
            {
                return NotFound();
            }

            var supplier = await _context.Suppliers
                .Include(s => s.City)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (supplier == null)
            {
                return NotFound();
            }

            return View(supplier);
        }

        // GET: Suppliers/Create
        public IActionResult Create()
        {
            ViewData["CityID"] = new SelectList(_context.Cities, "ID", "Name");
            return View();
        }

        // POST: Suppliers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,Name,Phone,Email,Address,Postal,CityID")] Supplier supplier)
        {
            if (ModelState.IsValid)
            {
                _context.Add(supplier);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CityID"] = new SelectList(_context.Cities, "ID", "Name", supplier.CityID);
            return View(supplier);
        }

        // GET: Suppliers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Suppliers == null)
            {
                return NotFound();
            }

            var supplier = await _context.Suppliers.FindAsync(id);
            if (supplier == null)
            {
                return NotFound();
            }
            ViewData["CityID"] = new SelectList(_context.Cities, "ID", "Name", supplier.CityID);
            return View(supplier);
        }

        // POST: Suppliers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,Name,Phone,Email,Address,Postal,CityID")] Supplier supplier)
        {
            if (id != supplier.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(supplier);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SupplierExists(supplier.ID))
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
            ViewData["CityID"] = new SelectList(_context.Cities, "ID", "Name", supplier.CityID);
            return View(supplier);
        }

        // GET: Suppliers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Suppliers == null)
            {
                return NotFound();
            }

            var supplier = await _context.Suppliers
                .Include(s => s.City)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (supplier == null)
            {
                return NotFound();
            }

            return View(supplier);
        }

        // POST: Suppliers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Suppliers == null)
            {
                return Problem("Entity set 'EmmasEnginesContext.Suppliers'  is null.");
            }
            var supplier = await _context.Suppliers.FindAsync(id);
            if (supplier != null)
            {
                _context.Suppliers.Remove(supplier);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SupplierExists(int id)
        {
          return _context.Suppliers.Any(e => e.ID == id);
        }
    }
}
