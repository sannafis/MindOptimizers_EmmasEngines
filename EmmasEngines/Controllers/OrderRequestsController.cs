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
    public class OrderRequestsController : Controller
    {
        private readonly EmmasEnginesContext _context;

        public OrderRequestsController(EmmasEnginesContext context)
        {
            _context = context;
        }

        // GET: OrderRequests
        public async Task<IActionResult> Index(string SearchString, string query, string actionButton, int? page, int? pageSizeID, string sortDirection = "asc", string sortField = "Name")
        {
            string[] sortOptions = new[] { "ID", "Description", "SentDate", "ReceiveDate", "ExternalOrderNum" };

            var emmasEnginesContext = from i in _context.OrderRequests
                  .Include(o => o.Customer)
                  .Include(o => o.OrderRequestInventories)
                    .ThenInclude(o => o.Inventory)
                  .AsNoTracking()
                                      select i;

            if (!String.IsNullOrEmpty(SearchString))
            {
                //Filter by UPC and Name
                emmasEnginesContext = emmasEnginesContext.Where(s => s.Description.ToUpper().Contains(SearchString.ToUpper()));
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
            if (sortField == "ID")
            {
                if (sortDirection == "asc")
                    emmasEnginesContext = emmasEnginesContext.OrderBy(s => s.ID);
                else
                    emmasEnginesContext = emmasEnginesContext.OrderByDescending(s => s.ID);
            }
			if (sortField == "InventoryName")
			{
				if (sortDirection == "asc")
					emmasEnginesContext = emmasEnginesContext.OrderBy(s => s.OrderRequestInventories.FirstOrDefault().Inventory.Name);
				else
					emmasEnginesContext = emmasEnginesContext.OrderByDescending(s => s.OrderRequestInventories.FirstOrDefault().Inventory.Name);
			}
			if (sortField == "Description")
            {
                if (sortDirection == "asc")
                    emmasEnginesContext = emmasEnginesContext.OrderBy(s => s.Description);
                else
                    emmasEnginesContext = emmasEnginesContext.OrderByDescending(s => s.Description);
            }
            else if (sortField == "SentDate")
            {
                if (sortDirection == "asc")
                    emmasEnginesContext = emmasEnginesContext.OrderBy(s => s.SentDate);
                else
                    emmasEnginesContext = emmasEnginesContext.OrderByDescending(s => s.SentDate);
            }
            else if (sortField == "ReceiveDate")
            {
                if (sortDirection == "asc")
                    emmasEnginesContext = emmasEnginesContext.OrderBy(s => s.ReceiveDate);
                else
                    emmasEnginesContext = emmasEnginesContext.OrderByDescending(s => s.ReceiveDate);
            }
            else if (sortField == "ExternalOrderNum")
            {
                if (sortDirection == "asc")
                    emmasEnginesContext = emmasEnginesContext.OrderBy(s => s.ExternalOrderNum);
                else
                    emmasEnginesContext = emmasEnginesContext.OrderByDescending(s => s.ExternalOrderNum);
            }
            //set sort for in ViewData
            ViewData["sortField"] = sortField;
            ViewData["sortDirection"] = sortDirection;

            int pageSize = PageSizeHelper.SetPageSize(HttpContext, pageSizeID, "Inventory");
            ViewData["pageSizeID"] = PageSizeHelper.PageSizeList(pageSize);

            var pagedData = await PaginatedList<OrderRequest>.CreateAsync(emmasEnginesContext.AsNoTracking(), page ?? 1, pageSize);

            return View(pagedData);
        }

        // GET: OrderRequests/Create
        public IActionResult Create()
        {
            ViewData["CustomerID"] = new SelectList(_context.Customers, "ID", "Address");
            ViewData["SupplierID"] = new SelectList(_context.Suppliers, "ID", "Address");
            return View();
        }

        // POST: OrderRequests/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,Description,SentDate,ReceiveDate,ExternalOrderNum,CustomerID,SupplierID")] OrderRequest orderRequest)
        {
            if (ModelState.IsValid)
            {
                _context.Add(orderRequest);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CustomerID"] = new SelectList(_context.Customers, "ID", "Address", orderRequest.CustomerID);
            ViewData["SupplierID"] = new SelectList(_context.Suppliers, "ID", "Address", orderRequest.SupplierID);
            return View(orderRequest);
        }


        /*************************************************************************************************/
        /*************************************************************************************************/
        //AddOrEdit get and post controller to combine this two actions

        // GET: OrderRequest/AddOrEdit
        // GET: OrderRequest/AddOrEdit/5
        public async Task<IActionResult> AddOrEdit(int id = 0)
        {
            if (id == 0)
            {
                ViewData["CustomerID"] = new SelectList(_context.Customers, "ID", "Address");
                ViewData["SupplierID"] = new SelectList(_context.Suppliers, "ID", "Address");
                return View(new OrderRequest());
            }
            else
            {
                var orderRequest = await _context.OrderRequests.FindAsync(id);
                if (orderRequest == null)
                {

                    return NotFound();
                }
                ViewData["CustomerID"] = new SelectList(_context.Customers, "ID", "Address", orderRequest.CustomerID);
                ViewData["SupplierID"] = new SelectList(_context.Suppliers, "ID", "Address", orderRequest.SupplierID);
                return View(orderRequest);
            }

        }

        // POST: OrderRequest/AddOrEdit
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddOrEdit(int id, [Bind("ID,Description,SentDate,ReceiveDate,ExternalOrderNum,CustomerID,SupplierID")] OrderRequest orderRequest)
        {
            if (ModelState.IsValid)
            {
                if (id == 0)
                {
                    _context.Add(orderRequest);
                    await _context.SaveChangesAsync();
                }
                else
                {
                    try
                    {
                        _context.Update(orderRequest);
                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!OrderRequestExists(orderRequest.ID))
                        {
                            return NotFound();
                        }
                        else
                        {
                            throw;
                        }
                    }
                }

                ViewData["CustomerID"] = new SelectList(_context.Customers, "ID", "Address", orderRequest.CustomerID);
                ViewData["SupplierID"] = new SelectList(_context.Suppliers, "ID", "Address", orderRequest.SupplierID);
                return Json(new { isValid = true, html = Helper.RenderRazorViewToString(this, "_ViewAll", _context.OrderRequests.ToList()) });
            }
            ViewData["CustomerID"] = new SelectList(_context.Customers, "ID", "Address", orderRequest.CustomerID);
            ViewData["SupplierID"] = new SelectList(_context.Suppliers, "ID", "Address", orderRequest.SupplierID);
            return Json(new { isValid = false, html = Helper.RenderRazorViewToString(this, "AddOrEdit", orderRequest) });
        }

        /*************************************************************************************************/
        /*************************************************************************************************/

        //GET: OrderRequests/Details/1
        public async Task<IActionResult> Details(int? id)
        {
            {
                if (id == null)
                {
                    return NotFound();
                }

                var orderRequest = await _context.OrderRequests
                    .Include(o => o.Customer)
                    .Include(o => o.Supplier)
                    .Include(o => o.OrderRequestInventories)
                        .ThenInclude(o => o.Inventory)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(m => m.ID == id);
                if (orderRequest == null)
                {
                    return NotFound();
                }

                return View(orderRequest);
            }
        }


		// GET: OrderRequests/Edit/5
		public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.OrderRequests == null)
            {
                return NotFound();
            }

            var orderRequest = await _context.OrderRequests.FindAsync(id);
            if (orderRequest == null)
            {
                return NotFound();
            }
            ViewData["CustomerID"] = new SelectList(_context.Customers, "ID", "Address", orderRequest.CustomerID);
            ViewData["SupplierID"] = new SelectList(_context.Suppliers, "ID", "Address", orderRequest.SupplierID);
            return View(orderRequest);
        }

        // POST: OrderRequests/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,Description,SentDate,ReceiveDate,ExternalOrderNum,CustomerID,SupplierID")] OrderRequest orderRequest)
        {
            if (id != orderRequest.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(orderRequest);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrderRequestExists(orderRequest.ID))
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
            ViewData["CustomerID"] = new SelectList(_context.Customers, "ID", "Address", orderRequest.CustomerID);
            ViewData["SupplierID"] = new SelectList(_context.Suppliers, "ID", "Address", orderRequest.SupplierID);
            return View(orderRequest);
        }

        // GET: OrderRequests/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.OrderRequests == null)
            {
                return NotFound();
            }

            var orderRequest = await _context.OrderRequests
                .Include(o => o.Customer)
                .Include(o => o.Supplier)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (orderRequest == null)
            {
                return NotFound();
            }

            return View(orderRequest);
        }

        // POST: OrderRequests/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.OrderRequests == null)
            {
                return Problem("Entity set 'EmmasEnginesContext.OrderRequests'  is null.");
            }
            var orderRequest = await _context.OrderRequests.FindAsync(id);
            if (orderRequest != null)
            {
                _context.OrderRequests.Remove(orderRequest);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool OrderRequestExists(int id)
        {
          return _context.OrderRequests.Any(e => e.ID == id);
        }
    }
}
