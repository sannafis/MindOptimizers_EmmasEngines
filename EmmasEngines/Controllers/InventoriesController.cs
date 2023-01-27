using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EmmasEngines.Data;
using EmmasEngines.Models;

namespace EmmasEngines.Controllers
{
    public class InventoriesController : Controller
    {
        private readonly EmmasEnginesContext _context;

        public InventoriesController(EmmasEnginesContext context)
        {
            _context = context;
        }

        // GET: Inventories
        public async Task<IActionResult> Index(string SearchString, string actionButton, string sortDirection = "asc", string sortField = "Name")
        {
            ViewData["Filtering"] = "";

            //sort options for the table (match column headings)
            string[] sortOptions = new[] { "UPC", "Name", "Size", "Quantity" };

            var inventories = from i in _context.Inventories
                              .AsNoTracking()
                              select i;

            if (!String.IsNullOrEmpty(SearchString))
            {
                //Filter by UPC and Name
                inventories = inventories.Where(s => s.Name.ToUpper().Contains(SearchString.ToUpper())
                                                   || s.UPC.ToUpper().Contains(SearchString.ToUpper()));
                ViewData["Filtering"] = "show";
            }

            if (!String.IsNullOrEmpty(actionButton))//check if form submitted
            {
                if (sortOptions.Contains(actionButton))
                {
                    if (actionButton == sortField)//reverse order  same column
                    {
                        sortDirection = (sortDirection == "asc" ? "desc" : "asc");
                    }
                    //sort by button clicked
                    sortField = actionButton;
                }
            }
            //Now sort by field and direction
            if (sortField == "UPC")
            {
                if (sortDirection == "asc")
                    inventories = inventories.OrderBy(s => s.UPC);
                else
                    inventories = inventories.OrderByDescending(s => s.UPC);
            }
            else if (sortField == "Name")
            {
                if (sortDirection == "asc")
                    inventories = inventories.OrderBy(s => s.Name);
                else
                    inventories = inventories.OrderByDescending(s => s.Name);
            }
            else if (sortField == "Size")
            {
                if (sortDirection == "asc")
                    inventories = inventories.OrderBy(s => s.Size);
                else
                    inventories = inventories.OrderByDescending(s => s.Size);
            }
            else if(sortField == "Quantity")
            {
                if (sortDirection == "asc")
                    inventories = inventories.OrderBy(s => s.Quantity);
                else
                    inventories = inventories.OrderByDescending(s => s.Quantity);
            }
            //set sort for in ViewData
            ViewData["sortField"] = sortField;
            ViewData["sortDirection"] = sortDirection;

            return View(await inventories.ToListAsync());
        }
       

        // GET: Inventories/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Inventories == null)
            {
                return NotFound();
            }

            var inventory = await _context.Inventories
                .FirstOrDefaultAsync(m => m.ID == id);
            if (inventory == null)
            {
                return NotFound();
            }

            return View(inventory);
        }

        // GET: Inventories/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Inventories/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,UPC,Name,Size,Quantity,Current")] Inventory inventory)
        {
            if (ModelState.IsValid)
            {
                _context.Add(inventory);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(inventory);
        }

        // GET: Inventories/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Inventories == null)
            {
                return NotFound();
            }

            var inventory = await _context.Inventories.FindAsync(id);
            if (inventory == null)
            {
                return NotFound();
            }
            return View(inventory);
        }

        // POST: Inventories/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,UPC,Name,Size,Quantity,Current")] Inventory inventory)
        {
            if (id != inventory.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(inventory);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!InventoryExists(inventory.ID))
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
            return View(inventory);
        }

        // GET: Inventories/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Inventories == null)
            {
                return NotFound();
            }

            var inventory = await _context.Inventories
                .FirstOrDefaultAsync(m => m.ID == id);
            if (inventory == null)
            {
                return NotFound();
            }

            return View(inventory);
        }

        // POST: Inventories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Inventories == null)
            {
                return Problem("Entity set 'EmmasEnginesContext.Inventories'  is null.");
            }
            var inventory = await _context.Inventories.FindAsync(id);
            if (inventory != null)
            {
                _context.Inventories.Remove(inventory);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool InventoryExists(int id)
        {
          return _context.Inventories.Any(e => e.ID == id);
        }
        
    }
}
