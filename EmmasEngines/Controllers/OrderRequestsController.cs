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
    public class OrderRequestsController : Controller
    {
        private readonly EmmasEnginesContext _context;

        public OrderRequestsController(EmmasEnginesContext context)
        {
            _context = context;
        }

        // GET: OrderRequests
        public async Task<IActionResult> Index()
        {
            var emmasEnginesContext = _context.OrderRequests.Include(o => o.Customer);
            return View(await emmasEnginesContext.ToListAsync());
        }

        // GET: OrderRequests/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.OrderRequests == null)
            {
                return NotFound();
            }

            var orderRequest = await _context.OrderRequests
                .Include(o => o.Customer)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (orderRequest == null)
            {
                return NotFound();
            }

            return View(orderRequest);
        }

        // GET: OrderRequests/Create
        public IActionResult Create()
        {
            ViewData["CustomerID"] = new SelectList(_context.Customers, "ID", "Address");
            return View();
        }

        // POST: OrderRequests/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,Description,SentDate,ReceiveDate,ExternalOrderNum,CustomerID")] OrderRequest orderRequest)
        {
            if (ModelState.IsValid)
            {
                _context.Add(orderRequest);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CustomerID"] = new SelectList(_context.Customers, "ID", "Address", orderRequest.CustomerID);
            return View(orderRequest);
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
            return View(orderRequest);
        }

        // POST: OrderRequests/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,Description,SentDate,ReceiveDate,ExternalOrderNum,CustomerID")] OrderRequest orderRequest)
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
