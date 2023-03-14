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
using NuGet.Protocol;

namespace EmmasEngines.Controllers
{
    public class InventoriesController : Controller
    {
        private readonly EmmasEnginesContext _context;

        public InventoriesController(EmmasEnginesContext context)
        {
            _context = context;
        }

        //Add includes to prices
            //error on: "sequences contains no elements" for "AdjustedPrices"
        [HttpPost]
        public JsonResult SearchInventory(string SearchString ="")
        {
            Console.WriteLine("SearchString: " + SearchString);
            ViewData["Filtering"] = "";
            if (!String.IsNullOrEmpty(SearchString))
            {
                var inventories = from i in _context.Inventories
                                  .Where(s => s.Name.ToUpper().Contains(SearchString.ToUpper())
                                  || s.UPC.ToUpper().Contains(SearchString.ToUpper()))
                                  select i;
              
                ViewData["Filtering"] = "show";

                return Json(inventories.ToList().FirstOrDefault());
            }
            else
            {
                return Json(null);
            }
        }


        // GET: Inventories
        public async Task<IActionResult> Index(string SearchString,string query, string actionButton, int? page, int? pageSizeID, string sortDirection = "asc", string sortField = "Name")
        {
            ViewData["Filtering"] = "";

            //sort options for the table (match column headings)
            string[] sortOptions = new[] { "UPC", "Name", "Size", "Quantity", "Cost (Avg)", "Price (Retail)" };

            //inventory list async
            var inventories = _context.Inventories

               .Include(i => i.Prices)
               .Include(o => o.OrderRequestInventories)
               .AsQueryable();



            if (!String.IsNullOrEmpty(SearchString))
            {
                //Filter by UPC and Name
                inventories = inventories.Where(s => s.Name.ToUpper().Contains(SearchString.ToUpper())
                                                   || s.UPC.ToUpper().Contains(SearchString.ToUpper()));
                ViewData["Filtering"] = "show";
                ViewData["Search Inventory"] = SearchString;
            }

            if (!String.IsNullOrEmpty(actionButton))//check if form submitted
            {
                page = 1;//reset page to 1
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
            else if (sortField == "Price (Retail)")
            {
                if (sortDirection == "asc")
                    inventories = inventories.OrderBy(s => Math.Round(((s.Prices.Select(p => p.PurchasedCost).Average() * 0.23) + s.Prices.Select(p => p.PurchasedCost).Average()), 2));
                else
                    inventories = inventories.OrderByDescending(s => Math.Round(((s.Prices.Select(p => p.PurchasedCost).Average() * 0.23) + s.Prices.Select(p => p.PurchasedCost).Average()), 2));
            }
            else if (sortField == "Cost (Avg)")
            {
                if (sortDirection == "asc")
                    inventories = inventories.OrderBy(s => s.Prices.Select(p => p.PurchasedCost).Average());
                else
                    inventories = inventories.OrderByDescending(s => s.Prices.Select(p => p.PurchasedCost).Average());
            }


            //set sort for in ViewData
            ViewData["sortField"] = sortField;
            ViewData["sortDirection"] = sortDirection;

            int pageSize = PageSizeHelper.SetPageSize(HttpContext, pageSizeID, "Inventory");
            ViewData["pageSizeID"] = PageSizeHelper.PageSizeList(pageSize);
            
            var pagedData = await PaginatedList<Inventory>.CreateAsync(inventories, page ?? 1, pageSize);

            return View(pagedData);
        }
       

        // GET: Inventories/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Inventories == null)
            {
                return NotFound();
            }

            var inventory = await _context.Inventories
                .Include(i => i.Prices)
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
        public async Task<IActionResult> Create([Bind("ID,UPC,Name,Size,Quantity,Current,SupplierID")] Inventory inventory)
        {
            if (ModelState.IsValid)
            {
                _context.Add(inventory);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(inventory);
        }

        /*************************************************************************************************/
        /*************************************************************************************************/
        //AddOrEdit get and post controller to combine this two actions

        // GET: Inventories/AddOrEdit
        // GET: Inventories/AddOrEdit/5
        public async Task <IActionResult> AddOrEdit(int id=0)
        {
            if (id==0)
            {
                return View(new Inventory());
            }
            else
            {
                var inventory = await _context.Inventories.FindAsync(id);
                if (inventory == null)
                {
                    return NotFound();
                }
                return View(inventory);
            }

        }

        // POST: Inventories/AddOrEdit
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddOrEdit(int id,[Bind("ID,UPC,Name,Size,Quantity,Current  ")] Inventory inventory)
        {
            if (ModelState.IsValid)
            {
                if (id == 0)
                {
                    _context.Add(inventory);
                    await _context.SaveChangesAsync();
                }
                else
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
                }


                return Json(new{isValid = true, html = Helper.RenderRazorViewToString(this, "_ViewAll", _context.Inventories.ToList()) });
            }
            return Json(new { isValid = false, html = Helper.RenderRazorViewToString(this, "AddOrEdit", inventory) });
        }

        /*************************************************************************************************/
        /*************************************************************************************************/

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
