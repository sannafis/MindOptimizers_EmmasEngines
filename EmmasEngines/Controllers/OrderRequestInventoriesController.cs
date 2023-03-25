using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EmmasEngines.Data;
using EmmasEngines.Models;
using Microsoft.AspNetCore.Authorization;

namespace EmmasEngines.Controllers
{
    [Authorize(Roles = "Admin,Supervisor")]
    public class OrderRequestInventoriesController : Controller
    {
        private readonly EmmasEnginesContext _context;

        public OrderRequestInventoriesController(EmmasEnginesContext context)
        {
            _context = context;
        }

        // GET: OrderRequestInventories
        public async Task<IActionResult> Index()
        {
            var emmasEnginesContext = _context.OrderRequestInventories.Include(o => o.Inventory).Include(o => o.OrderRequest);
            return View(await emmasEnginesContext.ToListAsync());
        }

        // GET: OrderRequestInventories/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.OrderRequestInventories == null)
            {
                return NotFound();
            }

            var orderRequestInventory = await _context.OrderRequestInventories
                .Include(o => o.Inventory)
                .Include(o => o.OrderRequest)
                .FirstOrDefaultAsync(m => m.OrderRequestID == id);
            if (orderRequestInventory == null)
            {
                return NotFound();
            }

            return View(orderRequestInventory);
        }

        // GET: OrderRequestInventories/Create
        public IActionResult Create()
        {
            ViewData["InventoryUPC"] = new SelectList(_context.Inventories, "UPC", "Name");
            ViewData["OrderRequestID"] = new SelectList(_context.OrderRequests, "ID", "Description");
            return View();
        }

        // POST: OrderRequestInventories/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("OrderRequestID,InventoryUPC,Quantity,Price")] OrderRequestInventory orderRequestInventory)
        {
            if (ModelState.IsValid)
            {
                _context.Add(orderRequestInventory);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["InventoryUPC"] = new SelectList(_context.Inventories, "UPC", "Name", orderRequestInventory.InventoryUPC);
            ViewData["OrderRequestID"] = new SelectList(_context.OrderRequests, "ID", "Description", orderRequestInventory.OrderRequestID);
            return View(orderRequestInventory);
        }

        // GET: OrderRequestInventories/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.OrderRequestInventories == null)
            {
                return NotFound();
            }

            var orderRequestInventory = await _context.OrderRequestInventories.FindAsync(id);
            if (orderRequestInventory == null)
            {
                return NotFound();
            }
            ViewData["InventoryUPC"] = new SelectList(_context.Inventories, "UPC", "Name", orderRequestInventory.InventoryUPC);
            ViewData["OrderRequestID"] = new SelectList(_context.OrderRequests, "ID", "Description", orderRequestInventory.OrderRequestID);
            return View(orderRequestInventory);
        }

        // POST: OrderRequestInventories/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("OrderRequestID,InventoryUPC,Quantity,Price")] OrderRequestInventory orderRequestInventory)
        {
            if (id != orderRequestInventory.OrderRequestID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(orderRequestInventory);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrderRequestInventoryExists(orderRequestInventory.OrderRequestID))
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
            ViewData["InventoryUPC"] = new SelectList(_context.Inventories, "UPC", "Name", orderRequestInventory.InventoryUPC);
            ViewData["OrderRequestID"] = new SelectList(_context.OrderRequests, "ID", "Description", orderRequestInventory.OrderRequestID);
            return View(orderRequestInventory);
        }

        // GET: OrderRequestInventories/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.OrderRequestInventories == null)
            {
                return NotFound();
            }

            var orderRequestInventory = await _context.OrderRequestInventories
                .Include(o => o.Inventory)
                .Include(o => o.OrderRequest)
                .FirstOrDefaultAsync(m => m.OrderRequestID == id);
            if (orderRequestInventory == null)
            {
                return NotFound();
            }

            return View(orderRequestInventory);
        }

        // POST: OrderRequestInventories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.OrderRequestInventories == null)
            {
                return Problem("Entity set 'EmmasEnginesContext.OrderRequestInventories'  is null.");
            }
            var orderRequestInventory = await _context.OrderRequestInventories.FindAsync(id);
            if (orderRequestInventory != null)
            {
                _context.OrderRequestInventories.Remove(orderRequestInventory);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool OrderRequestInventoryExists(int id)
        {
          return _context.OrderRequestInventories.Any(e => e.OrderRequestID == id);
        }
    }
}
