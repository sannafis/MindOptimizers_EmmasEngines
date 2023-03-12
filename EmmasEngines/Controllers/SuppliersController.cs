using System;
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
        public async Task<IActionResult> Index(string SearchString, string query, string actionButton, int? page, int? pageSizeID, string sortDirection = "asc",  string sortField = "Name")
        {
            //List of sort options
            string[] sortOptions = new[] { "ID", "Name" }; 
            
            var suppliers = _context.Suppliers
                .Include(s => s.City)
                .AsNoTracking();

            if (!String.IsNullOrEmpty(SearchString))
            {
                suppliers = suppliers.Where(s => s.Name.ToLower().Contains(SearchString.ToLower())
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
                    suppliers = suppliers.OrderBy(s => s.Name);
                }
                else
                {
                    suppliers = suppliers.OrderByDescending(s => s.Name);
                }
            }
            else
            {
                if(sortDirection == "asc")
                {
                    suppliers = suppliers
                        .OrderBy(Supplier => Supplier.ID);
                }
                else
                {
                    suppliers = suppliers
                        .OrderByDescending(Supplier => Supplier.ID);
                }
            }
            //Set Sort for next time
            ViewData["sortField"] = sortField;
            ViewData["sortDirection"] = sortDirection;


            int pageSize = PageSizeHelper.SetPageSize(HttpContext, pageSizeID, "Supplier");
            ViewData["pageSizeID"] = PageSizeHelper.PageSizeList(pageSize);

            var pagedData = await PaginatedList<Supplier>.CreateAsync(suppliers.AsNoTracking(), page ?? 1, pageSize);
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

        /*************************************************************************************************/
        /*************************************************************************************************/
        //AddOrEdit get and post controller to combine this two actions

        // GET: Suppliers/AddOrEdit
        // GET: Suppliers/AddOrEdit/5
        public async Task<IActionResult> AddOrEdit(int id = 0)
        {
            if (id == 0)
            {
                ViewData["CityID"] = new SelectList(_context.Cities, "ID", "Name");
                return View(new Supplier());    
            }
            else
            {
                var supplier = await _context.Suppliers.FindAsync(id);
                if (supplier == null)
                {
                    return NotFound();
                }
                ViewData["CityID"] = new SelectList(_context.Cities, "ID", "Name", supplier.CityID);
                return View(supplier);
            }

        }

        // POST: Suppliers/AddOrEdit
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddOrEdit(int id, [Bind("ID,Name,Phone,Email,Address,Postal,CityID")] Supplier supplier)
        {
            if (ModelState.IsValid)
            {
                if (id == 0)
                {
                    _context.Add(supplier);
                    await _context.SaveChangesAsync();
                }
                else
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
                }

                ViewData["CityID"] = new SelectList(_context.Cities, "ID", "Name", supplier.CityID);
                return Json(new { isValid = true, html = Helper.RenderRazorViewToString(this, "Index", _context.Suppliers.ToList()) });
            }
            ViewData["CityID"] = new SelectList(_context.Cities, "ID", "Name", supplier.CityID);
            return Json(new { isValid = false, html = Helper.RenderRazorViewToString(this, "AddOrEdit", supplier) });
        }

        /*************************************************************************************************/
        /*************************************************************************************************/

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
