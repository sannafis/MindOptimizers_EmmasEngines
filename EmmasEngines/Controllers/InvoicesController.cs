using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EmmasEngines.Data;
using EmmasEngines.Models;
using System.Diagnostics;

namespace EmmasEngines.Controllers
{
    public class InvoicesController : Controller
    {
        private readonly EmmasEnginesContext _context;

        public InvoicesController(EmmasEnginesContext context)
        {
            _context = context;
        }

        // GET: Invoices
        public async Task<IActionResult> Index()
        {
            var emmasEnginesContext = _context.Invoices
                .Include(i => i.Customer)
                .Include(i => i.Employee)
                .Include(i => i.InvoiceLines)
                .Include(i => i.InvoicePayments);
            return View(await emmasEnginesContext.ToListAsync());
        }

        // GET: Invoices/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Invoices == null)
            {
                return NotFound();
            }

            var invoice = await _context.Invoices
                .Include(i => i.Customer)
                .Include(i => i.Employee)
                .Include(i => i.InvoiceLines)
                .Include(i => i.InvoicePayments)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (invoice == null)
            {
                return NotFound();
            }

            return View(invoice);
        }

        // GET: Invoices/Create
        public IActionResult Create()
        {
            ViewData["CustomerID"] = new SelectList(_context.Customers, "ID", "FullName");
            ViewData["EmployeeID"] = new SelectList(_context.Employees, "ID", "FirstName");
            return View();
        }

        // POST: Invoices/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        [Route("Invoices/Create")]
        public async Task<IActionResult> Create([FromBody] Invoice invoice)
        {
            if (ModelState.IsValid)
            {
                //Get the data to construct a complete invoice object
                //This includes: invoice, invoice lines, invoice payments

                var session = HttpContext.Session;
                var invoiceLines = Utilities.SessionExtensions.GetObjectFromJson<List<InvoiceLine>>(session, "invoiceLines");// Get the invoice lines from the session

                if (invoiceLines == null || invoiceLines.Count == 0)
                {
                    // If there are no invoice lines, return an error
                    return BadRequest("No invoice lines found");
                }

                Int32.TryParse(HttpContext.Session.GetString("CustomerID"), out int customerID);
                if (customerID == 0)
                {
                    //return BadRequest("Customer must be selected!");
                    customerID = 1;
                }
                var invoiceToAdd = new Invoice
                {
                    Date = DateTime.UtcNow,
                    Appreciation = invoice.Appreciation,
                    Description = invoice.Description,
                    CustomerID = customerID, // Replace with actual customer ID
                    EmployeeID = invoice.EmployeeID, // Replace with actual employee ID
                    InvoiceLines = new List<InvoiceLine>(),
                    InvoicePayments = new List<InvoicePayment>()
                };


                // Add the invoice to the context
                _context.Invoices.Add(invoiceToAdd);

                // Add the invoice lines to the invoice
                foreach (var line in invoiceLines)
                {
                    Debug.WriteLine($"Creating new invoice line for inventory UPC: {line.InventoryUPC}...");

                    var invoiceLineToAdd = new InvoiceLine
                    {
                        Quantity = line.Quantity,
                        SalePrice = line.SalePrice,
                        InventoryUPC = line.InventoryUPC,
                        InvoiceID = invoiceToAdd.ID,
                        Invoice = invoiceToAdd
                    };

                    // Add the invoice line to the context and the invoice
                    invoiceToAdd.InvoiceLines.Add(invoiceLineToAdd);
                    _context.InvoiceLines.Add(invoiceLineToAdd);
                }
                if(invoiceLines.Count == 0)
                {
                    var invoiceLineToAdd = new InvoiceLine
                    {
                        Quantity = 1,
                        SalePrice = 5,
                        InventoryUPC = "06059100",
                        InvoiceID = invoiceToAdd.ID,
                        Invoice = invoiceToAdd
                    };
                    // Add the invoice line to the context and the invoice
                    invoiceToAdd.InvoiceLines.Add(invoiceLineToAdd);
                    _context.InvoiceLines.Add(invoiceLineToAdd);
                }

                // Add the invoice payments to the invoice
                foreach (var invoicePayment in invoice.InvoicePayments)
                {
                    // Create a new payment object
                    var invoicePaymentToAdd = new InvoicePayment
                    {
                        PaymentID = invoicePayment.PaymentID,//Get the payment ID from the form data
                        Invoice = invoiceToAdd,
                        InvoiceID = invoiceToAdd.ID
                    };
                    
                    invoiceToAdd.InvoicePayments.Add(invoicePaymentToAdd);// Add the invoice payment to the invoice
                    _context.InvoicePayments.Add(invoicePaymentToAdd);// Add the invoice payment to the context
                }

                await _context.SaveChangesAsync();
                //// Return the complete invoice object including invoice lines and invoice payments
                var completeInvoice = await _context.Invoices
                    .Include(i => i.Customer)
                    .Include(i => i.Employee)
                    .Include(i => i.InvoiceLines)
                    .Include(i => i.InvoicePayments)
                    .FirstOrDefaultAsync(m => m.ID == invoiceToAdd.ID);
                return Json(completeInvoice);
            }

            return Json(new { success = false, errors = ModelState.Values.SelectMany(v => v.Errors) });
        }


        // GET: Invoices/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Invoices == null)
            {
                return NotFound();
            }

            var invoice = await _context.Invoices.FindAsync(id);
            if (invoice == null)
            {
                return NotFound();
            }
            ViewData["CustomerID"] = new SelectList(_context.Customers, "ID", "Address", invoice.CustomerID);
            ViewData["EmployeeID"] = new SelectList(_context.Employees, "ID", "FirstName", invoice.EmployeeID);
            return View(invoice);
        }

        // POST: Invoices/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,Date,Appreciation,Description,CustomerID,EmployeeID")] Invoice invoice)
        {
            if (id != invoice.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(invoice);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!InvoiceExists(invoice.ID))
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
            ViewData["CustomerID"] = new SelectList(_context.Customers, "ID", "Address", invoice.CustomerID);
            ViewData["EmployeeID"] = new SelectList(_context.Employees, "ID", "FirstName", invoice.EmployeeID);
            return View(invoice);
        }

        // GET: Invoices/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Invoices == null)
            {
                return NotFound();
            }

            var invoice = await _context.Invoices
                .Include(i => i.Customer)
                .Include(i => i.Employee)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (invoice == null)
            {
                return NotFound();
            }

            return View(invoice);
        }

        // POST: Invoices/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Invoices == null)
            {
                return Problem("Entity set 'EmmasEnginesContext.Invoices'  is null.");
            }
            var invoice = await _context.Invoices.FindAsync(id);
            if (invoice != null)
            {
                _context.Invoices.Remove(invoice);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool InvoiceExists(int id)
        {
          return _context.Invoices.Any(e => e.ID == id);
        }
    }
}
