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
    public class CustomersController : Controller
    {
        private readonly EmmasEnginesContext _context;

        public CustomersController(EmmasEnginesContext context)
        {
            _context = context;
        }

        // GET: Customers
        public async Task<IActionResult> Index(string SearchString, string actionButton, int? page, int? pageSizeID, string sortDirection = "asc", string sortField = "Name")
        {
            //List of sort options
            string[] sortOptions = new[] { "ID", "First Name", "Last Name" };



            var emmasEnginesContext = _context.Customers
                .Include(c => c.City)
                .AsNoTracking();
            
            if (!String.IsNullOrEmpty(SearchString))
            {
                emmasEnginesContext = emmasEnginesContext.Where(
                    s => s.FirstName.ToLower().Contains(SearchString.ToLower()) ||
                         s.LastName.ToLower().Contains(SearchString.ToLower()) ||
                         s.City.Name.ToLower().Contains(SearchString.ToLower()));
                ViewData["Search Customers"] = SearchString;
            }

            //Sorting
            if (!String.IsNullOrEmpty(actionButton)) //Form submitted
            {
                page = 1; // Reset to page 1
                if (sortOptions.Contains(actionButton)) //Change of sort is requested
                {
                    if (actionButton == sortField) //Reverse order on same field
                    {
                        sortDirection = sortDirection == "asc" ? "desc" : "asc";
                    }
                    sortField = actionButton; //Sort by the button clicked
                }
            }

            //Which field and direction to sort by
            if (sortField == "Last Name")
            {
                if(sortDirection == "asc")
                {
                    emmasEnginesContext = emmasEnginesContext
                        .OrderBy(Customer => Customer.LastName);
                }
                else
                {
                    emmasEnginesContext = emmasEnginesContext
                        .OrderByDescending(Customer => Customer.LastName);
                }
            }
            else if (sortField == "First Name")
            {
                if (sortDirection == "asc")
                {
                    emmasEnginesContext = emmasEnginesContext
                        .OrderBy(Customer => Customer.FirstName);
                }
                else
                {
                    emmasEnginesContext = emmasEnginesContext
                        .OrderByDescending(Customer => Customer.FirstName);
                }
            }
            else
            {
                if (sortDirection == "asc")
                {
                    emmasEnginesContext = emmasEnginesContext
                        .OrderBy(c => c.ID);
                }
                else
                {
                    emmasEnginesContext = emmasEnginesContext
                        .OrderByDescending(Customer => Customer.ID);
                }
                
            }
            //Set Sort for next time
            ViewData["sortField"] = sortField;
            ViewData["sortDirection"] = sortDirection;


            int pageSize = PageSizeHelper.SetPageSize(HttpContext, pageSizeID, "Customer");
            ViewData["pageSizeID"] = PageSizeHelper.PageSizeList(pageSize);

            var pagedData = await PaginatedList<Customer>.CreateAsync(emmasEnginesContext.AsNoTracking(), page ?? 1, pageSize);
            //return View(await emmasEnginesContext.ToListAsync());
            return View(pagedData);

            //return View(await emmasEnginesContext.ToListAsync());
        }

        // GET: Customers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Customers == null)
            {
                return NotFound();
            }

            var customer = await _context.Customers
                .Include(c => c.City)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (customer == null)
            {
                return NotFound();
            }

            return View(customer);
        }

        // GET: Customers/Create
        public IActionResult Create()
        {
            ViewData["CityID"] = new SelectList(_context.Cities, "ID", "Name");
            return View();
        }

        // POST: Customers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,FirstName,LastName,Phone,Address,Postal,CityID")] Customer customer)
        {
            if (ModelState.IsValid)
            {
                _context.Add(customer);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CityID"] = new SelectList(_context.Cities, "ID", "Name", customer.CityID);
            return View(customer);
        }

        /*************************************************************************************************/
        /*************************************************************************************************/
        //AddOrEdit get and post controller to combine this two actions

        // GET: Customers/AddOrEdit
        // GET: Customers/AddOrEdit/5
        public async Task<IActionResult> AddOrEdit(int id = 0)
        {
            if (id == 0)
            {
                ViewData["CityID"] = new SelectList(_context.Cities, "ID", "Name");
                return View(new Customer());
            }
            else
            {
                var customer = await _context.Inventories.FindAsync(id);
                if (customer == null)
                {
                    return NotFound();
                }
                ViewData["CityID"] = new SelectList(_context.Cities, "ID", "Name");
                return View(customer);
            }

        }

        // POST: Customers/AddOrEdit
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddOrEdit(int id, [Bind("ID,FirstName,LastName,Phone,Address,Postal,CityID")] Customer customer)
        {
            if (ModelState.IsValid)
            {
                if (id == 0)
                {
                    _context.Add(customer);
                    await _context.SaveChangesAsync();
                }
                else
                {
                    try
                    {
                        _context.Update(customer);
                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!CustomerExists(customer.ID))
                        {
                            return NotFound();
                        }
                        else
                        {
                            throw;
                        }
                    }
                }

                ViewData["CityID"] = new SelectList(_context.Cities, "ID", "Name");
                return Json(new { isValid = true, html = Helper.RenderRazorViewToString(this, "_ViewAll", _context.Customers.ToList()) });
            }
            ViewData["CityID"] = new SelectList(_context.Cities, "ID", "Name");
            return Json(new { isValid = false, html = Helper.RenderRazorViewToString(this, "AddOrEdit", customer) });
        }

        /*************************************************************************************************/
        /*************************************************************************************************/

        // GET: Customers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Customers == null)
            {
                return NotFound();
            }

            var customer = await _context.Customers.FindAsync(id);
            if (customer == null)
            {
                return NotFound();
            }
            ViewData["CityID"] = new SelectList(_context.Cities, "ID", "Name", customer.CityID);
            return View(customer);
        }

        // POST: Customers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,FirstName,LastName,Phone,Address,Postal,CityID")] Customer customer)
        {
            if (id != customer.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(customer);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CustomerExists(customer.ID))
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
            ViewData["CityID"] = new SelectList(_context.Cities, "ID", "Name", customer.CityID);
            return View(customer);
        }

        // GET: Customers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Customers == null)
            {
                return NotFound();
            }

            var customer = await _context.Customers
                .Include(c => c.City)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (customer == null)
            {
                return NotFound();
            }

            return View(customer);
        }

        // POST: Customers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Customers == null)
            {
                return Problem("Entity set 'EmmasEnginesContext.Customers'  is null.");
            }
            var customer = await _context.Customers.FindAsync(id);
            if (customer != null)
            {
                _context.Customers.Remove(customer);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CustomerExists(int id)
        {
            return _context.Customers.Any(e => e.ID == id);
        }
    }
}
