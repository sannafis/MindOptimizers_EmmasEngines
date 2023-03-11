using EmmasEngines.Data;
using EmmasEngines.Models;
using EmmasEngines.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EmmasEngines.Controllers
{
    public class POSController : Controller
    {
        private readonly EmmasEnginesContext _context;
        private ISession session;

        public POSController(EmmasEnginesContext context)
        {
            _context = context;
           // session = HttpContext.Session;
        }

        [HttpPost]
        public JsonResult SearchInventory(string SearchString = "")
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

        [HttpPost]
        public PartialViewResult SearchCustomer(string SearchString = "")
        {
            Console.WriteLine("SearchString: " + SearchString);
            ViewData["Filtering"] = "";
            if (!String.IsNullOrEmpty(SearchString))
            {
                var customers = from c in _context.Customers
                                .Where(s => s.FirstName.ToUpper().Contains(SearchString.ToUpper())
                                || s.Phone.ToUpper().Contains(SearchString.ToUpper()))
                                select c;

                ViewData["Filtering"] = "show";

                return PartialView("_CustomerDetails", customers.FirstOrDefault());
            }
            else
            {
                return PartialView("_CustomerDetails", null);
            }
        }



        public async Task<IActionResult> Index(string SearchString, int? pageSizeID, int? page, string actionButton)
        {
            ViewData["Filtering"] = "";

            var inventories = from i in _context.Inventories
               // .Include(p => p.Prices)
                              select i;
            var customer = _context.Customers.FirstOrDefault();
            ViewData["Customer"] = customer;
            

            if (!String.IsNullOrEmpty(SearchString))
            {
                inventories = inventories.Where(s => s.Name.ToUpper().Contains(SearchString.ToUpper())
                || s.UPC.ToUpper().Contains(SearchString.ToUpper()));
                ViewData["Filtering"] = "show";
            }
            /*if (!String.IsNullOrEmpty(SearchString) && actionButton == "FilterCustomer")
            {
                var customers = from c in _context.Customers
                                .Where(s => s.FullName.ToUpper().Contains(SearchString.ToUpper())
                                || s.Phone.ToUpper().Contains(SearchString.ToUpper()))
                                select c;

                ViewData["Filtering"] = "show";
                //var session = HttpContext.Session;
                //Utilities.SessionExtensions.SetObjectAsJson(session, "customer", customers.FirstOrDefault());
            }*/

            int pageSize = PageSizeHelper.SetPageSize(HttpContext, pageSizeID, "POS");
            ViewData["pageSizeID"] = PageSizeHelper.PageSizeList(pageSize);
            

            var pagedData = await PaginatedList<Inventory>.CreateAsync(inventories.AsNoTracking(), page ?? 1, pageSize);

            return View(pagedData);
        }

        //Remove from cart
        public IActionResult RemoveFromCart(string UPC)
        {
            var session = HttpContext.Session;

            List<Inventory> cart = Utilities.SessionExtensions.GetObjectFromJson<List<Inventory>>(session, "cart");
            int index = isExist(UPC);
            cart.RemoveAt(index);
            Utilities.SessionExtensions.SetObjectAsJson(session, "cart", cart);
            return RedirectToAction("Index", "POS");
        }

        //Update cart
        public IActionResult UpdateCart(string UPC, int quantity)
        {
            var session = HttpContext.Session;

            List<Inventory> cart = Utilities.SessionExtensions.GetObjectFromJson<List<Inventory>>(session, "cart");
            int index = isExist(UPC);
            cart[index].Quantity = quantity.ToString();
            Utilities.SessionExtensions.SetObjectAsJson(session, "cart", cart);
            return RedirectToAction("Index", "POS");
        }

        //Check if item is in cart
        private int isExist(string UPC)
        {
            List<Inventory> cart = Utilities.SessionExtensions.GetObjectFromJson<List<Inventory>>(HttpContext.Session, "cart");
            for (int i = 0; i < cart.Count; i++)
            {
                if (cart[i].UPC.Equals(UPC))
                {
                    return i;
                }
            }
            return -1;
        }

        public ActionResult Buy(string UPC)
        {
            var inventory = _context.Inventories
                .Where(i => i.UPC == UPC)
               // .Include(p => p.Prices)
                .FirstOrDefault();

            if (inventory == null)
            {
                return NotFound();
            }

            var session = HttpContext.Session;

            if (session.GetString("invoiceLines") == null)
            {
                // If the cart doesn't exist, create a new list of invoice lines
                List<InvoiceLine> invoiceLines = new List<InvoiceLine>()
        {
            new InvoiceLine
            {
                InventoryUPC = inventory.UPC,
                Inventory = inventory,
                Quantity = 1,
               // SalePrice = inventory.Prices.Where(p => p.InventoryUPC == inventory.UPC).FirstOrDefault()?.PurchasedCost ?? 0.0
            }
        };

                Utilities.SessionExtensions.SetObjectAsJson(session, "invoiceLines", invoiceLines);
                AddInventoryItemToCart(UPC);
            }
            else
            {
                // If the cart already exists, check if the item is already in the cart
                List<InvoiceLine> invoiceLines = Utilities.SessionExtensions.GetObjectFromJson<List<InvoiceLine>>(session, "invoiceLines");
                var existingLine = invoiceLines.FirstOrDefault(l => l.InventoryUPC == inventory.UPC);

                if (existingLine != null)
                {
                    // If the item is already in the cart, increment the quantity of the existing line
                    existingLine.Quantity++;
                }
                else
                {
                    // If the item is not in the cart, add a new invoice line for the inventory item
                    invoiceLines.Add(new InvoiceLine
                    {
                        InventoryUPC = inventory.UPC,
                        Inventory = inventory,
                        Quantity = 1,
                       // SalePrice = inventory.Prices.Where(p => p.InventoryUPC == inventory.UPC).FirstOrDefault()?.PurchasedCost ?? 0.0
                    });
                }

                Utilities.SessionExtensions.SetObjectAsJson(session, "invoiceLines", invoiceLines);
                AddInventoryItemToCart(UPC);
            }

            return RedirectToAction("Index", "POS");
        }


        public void AddInventoryItemToCart(string UPC)
        {
            var inventories = from i in _context.Inventories
                             .Where(u => u.UPC == UPC)
                            // .Include(p => p.Prices)
                              select i;

            var session = HttpContext.Session;
            List<InvoiceLine> invoices = new List<InvoiceLine>();
            //for each cart item, add an InvoiceLine


            if (session.GetString("cart") == null)
            {
                List<Inventory> cart = new()
                {
                    inventories.FirstOrDefault()
                    
                };


                Utilities.SessionExtensions.SetObjectAsJson(session, "cart", inventories);
            }
            else
            {

                List<Inventory> cart = Utilities.SessionExtensions.GetObjectFromJson<List<Inventory>>(session, "cart");
                //if item with matching upc isnt already in cart, add it
                if (cart.FirstOrDefault(i => i.UPC == UPC) == null)
                    cart.Add(inventories.FirstOrDefault());

                Utilities.SessionExtensions.SetObjectAsJson(session, "cart", cart);
            }
        }

        //Add to cart
        /*public ActionResult Buy(string UPC)
        {
            var inventories = from i in _context.Inventories
                              .Where(u => u.UPC == UPC)
                              .Include(p => p.Prices)
                              select i;

            var session = HttpContext.Session;
            List < InvoiceLine > invoices = new List<InvoiceLine>();
            //for each cart item, add an InvoiceLine
            

            if (session.GetString("cart") == null)
            {
                List<Inventory> cart = new()
                {
                    inventories.FirstOrDefault()
                };
                
                
                Utilities.SessionExtensions.SetObjectAsJson(session, "cart", inventories);
                return RedirectToAction("Index", "POS");
            }
            else
            {

                List<Inventory> cart = Utilities.SessionExtensions.GetObjectFromJson<List<Inventory>>(session, "cart");
                cart.Add(inventories.FirstOrDefault());

                Utilities.SessionExtensions.SetObjectAsJson(session, "cart", cart);
                return RedirectToAction("Index", "POS");
            }

        }*/

        //Payment action
        //Submit: Create InvoiceLines for each item in the cart
        //Redirect to InvoiceController, passing the list of invoice lines
        public IActionResult Submit(string actionButton, string invoiceID)
        {
            var session = HttpContext.Session;

            List<Inventory> cart = Utilities.SessionExtensions.GetObjectFromJson<List<Inventory>>(session, "cart");
            var customer = Utilities.SessionExtensions.GetObjectFromJson<Customer>(session, "customer");

            List<InvoiceLine> invoiceLines = new();
            foreach (var item in cart)
            {
                InvoiceLine invoiceLine = new()
                {
                    InventoryUPC = item.UPC,
                    Quantity = Convert.ToInt32(item.Quantity),
                    SalePrice = item.MarkupPrice,
                    // HOW TO GET INVOICE?
                    //InvoiceID = ??
                };
                invoiceLines.Add(invoiceLine);
            }
            return RedirectToAction("Create", "Invoices", invoiceLines);
        }

    }
}
