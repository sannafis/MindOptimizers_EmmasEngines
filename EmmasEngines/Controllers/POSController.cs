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

        public async Task<IActionResult> Index(string SearchString, int? pageSizeID, int? page, string actionButton)
        {
            ViewData["Filtering"] = "";

            var inventories = from i in _context.Inventories
                .Include(p => p.Prices)
                              select i;

            if (!String.IsNullOrEmpty(SearchString))
            {
                inventories = inventories.Where(s => s.Name.ToUpper().Contains(SearchString.ToUpper())
                || s.UPC.ToUpper().Contains(SearchString.ToUpper()));
                ViewData["Filtering"] = "show";
            }
            int pageSize = PageSizeHelper.SetPageSize(HttpContext, pageSizeID, "POS");
            ViewData["pageSizeID"] = PageSizeHelper.PageSizeList(pageSize);

            var pagedData = await PaginatedList<Inventory>.CreateAsync(inventories.AsNoTracking(), page ?? 1, pageSize);

            return View(pagedData);
        }
               

        public ActionResult Buy(string UPC)
        {
            var inventories = from i in _context.Inventories
                              .Where(u => u.UPC == UPC)
                              .Include(p => p.Prices)
                              select i;

            var session = HttpContext.Session;

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
                Utilities.SessionExtensions.SetObjectAsJson(session, "cart", inventories);
                return RedirectToAction("Index", "POS");
            }
        }
        




    }
}
