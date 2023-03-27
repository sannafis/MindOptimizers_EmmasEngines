using Microsoft.AspNetCore.Mvc;
using EmmasEngines.Data;
using EmmasEngines.Models;
using EmmasEngines.Utilities;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using EmmasEngines.ViewModels;
using iText.Kernel.Geom;
using System.Drawing.Printing;
using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Layout.Element;
using iText.Layout.Properties;
using iText.Layout;
using System.IO;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Table = iText.Layout.Element.Table;
using Microsoft.Extensions.Hosting;
using Path = System.IO.Path;

namespace EmmasEngines.Controllers
{
    [Authorize(Roles = "Admin,Supervisor, Staff")]
    public class ReportsController : Controller
    {
        //add db context
        private readonly EmmasEnginesContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;

        public ReportsController(EmmasEnginesContext context, IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            _hostEnvironment = hostEnvironment;
        }

        public async Task<IActionResult> Index(int? page, int? pageSize)
        {
            //set default values for page and page size
            page ??= 1;
            if (int.TryParse(Request.Query["pageSize"], out int parsedPageSize))
            {
                pageSize = parsedPageSize;
            }
            pageSize = pageSize == 0 ? PageSizeHelper.SetPageSize(HttpContext, null, "Reports") : pageSize;


            // Get paginated list of saved sales reports
            var savedReports = await PaginatedList<SalesReport>.CreateAsync(_context.SalesReports.AsQueryable(), page ?? 1, pageSize ?? 5);
            // get report list of saved sales 
            var savedSalesReports = await PaginatedList<Report>.CreateAsync(_context.Reports.Where(i => i.Type == ReportType.Sales), page ?? 1, pageSize ?? 5);

            // Get paginated list of hourly reports
            var hourlyReports = await PaginatedList<HourlyReport>.CreateAsync(_context.HourlyReports.AsQueryable(), page ?? 1, pageSize ?? 5);
            var savedHourlyReports = await PaginatedList<Report>.CreateAsync(_context.Reports.Where(i => i.Type == ReportType.Hourly), page ?? 1, pageSize ?? 5);

            //Get paginated list of COGS reports
            /*
            var COGSReports = await PaginatedList<COGSReport>.CreateAsync(_context.COGSReports.AsQueryable(), page ?? 1, pageSize ?? 5);
            var savedCOGSReports = await PaginatedList<Report>.CreateAsync(_context.Reports.Where(i => i.Type == ReportType.COGS), page ?? 1, pageSize ?? 5);
            */

            
            // Create a view model to pass to the view
            var viewModel = new ReportsVM
            {
                SavedSalesReports = savedReports,
                SalesReportVM = new SalesReportVM
                {
                    SavedSalesReports = savedSalesReports,
                    Employees = await _context.Employees.ToListAsync()
                },
                SavedHourlyReports = hourlyReports,
                HourlyReportVM = new HourlyReportVM
                {
                    SavedHourlyReports = savedHourlyReports,
                    Employees = await _context.Employees.ToListAsync()
                },
                /*SavedCOGSReports = savedCOGSReports,
                COGSReportsVM = new COGSReportVM
                {
                    SavedCOGSReports = savedCOGSReports,
                    Inventories = await _context.Inventories.ToListAsync(),
                    Invoices = await _context.Invoices.ToListAsync()
                },*/
                PageIndex = savedReports.PageIndex,
                PageSize = savedReports.Count,
                TotalPages = savedReports.TotalPages
            };


            return View(viewModel);
        }

        #region Sales Report

        //Sales Report
        public async Task<IActionResult> Sales()
        {
            var viewModel = new SalesReportVM
            {
                SavedReports = await _context.SalesReports.Include(sr => sr.SalesReportEmployees).ThenInclude(sre => sre.Employee).ToListAsync(),
                Employees = await _context.Employees.ToListAsync(),
                NewReport = new NewSalesReport()
            };



            return View(viewModel);
        }
        /*
        //COGS Report
        public async Task<IActionResult> COGS()
        {
            var viewModel = new COGSReportVM
            {
                SavedReports = await _context.COGSReports.Include(sr => sr.COGSReportInventories).ThenInclude(sre => sre.Inventory).ToListAsync(),
                Inventories = await _context.Inventories.ToListAsync(),
                Invoices = await _context.Invoices.ToListAsync(),
                NewReport = new NewCOGSReport()
            };

            return View(viewModel);
        }
        */

            [HttpPost]
        public async Task<IActionResult> CreateSaleReport([FromForm] NewSalesReport newReport, int? page, int? pageSize)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { success = false, message = "Invalid form data." });
            }

            try
            {
                // Check the input parameters for null values
                page ??= 1;
                pageSize ??= 5;
                // Generate the report data (payment type summary, tax summary, employee summary, sales summary, appreciation)
                // based on the selected employee(s) and date range
                var salesData = _context.Invoices
                    .Include(s => s.Employee)
                    .Include(s => s.InvoicePayments)
                    .ThenInclude(s => s.Payment)
                    .Include(s => s.InvoiceLines)
                    .ThenInclude(s => s.Inventory)
                    .Where(s => newReport.StartDate <= s.Date && s.Date <= newReport.EndDate)
                    .AsQueryable();

                if (!newReport.AllEmployees && newReport.EmployeeId.HasValue)
                {
                    salesData = salesData.Where(s => s.Employee.ID == newReport.EmployeeId.Value);
                }

                var salesDataList = await salesData.ToListAsync();

                // Calculate the payment type summary
                var paymentTypeSummary = salesDataList.GroupBy(s => s.InvoicePayments.FirstOrDefault().Payment.Type)
                    .Select(g => new
                    {
                        PaymentType = g.Key,
                        Amount = g.Sum(s => s.Subtotal),
                        Count = g.Count()
                    }).ToList();

                // Calculate the tax summary (%13 of subottoal) provide a totaled value, and a field for other taxes incase we add them later
                // the key is tax type not payment type
                var taxSummary = salesDataList.GroupBy(s => s.InvoicePayments.FirstOrDefault().Payment.Type)
                    .Select(g => new
                    {
                        PaymentType = g.Key,
                        Amount = g.Sum(s => s.Subtotal * .13),
                        Count = g.Count()
                    }).ToList();

                // Calculate the employee summary
                var employeeSummary = salesDataList.GroupBy(s => s.Employee.ID)
                    .Select(g => new
                    {
                        EmployeeID = g.Key,
                        Sales = g.Sum(s => s.Subtotal)
                    }).ToList();

                // Calculate the sales summary
                var salesSummary = salesDataList.GroupBy(s => s.InvoiceLines.FirstOrDefault().Inventory.UPC)
                    .Select(g => new
                    {
                        InventoryUPC = g.Key,
                        Price = g.First().InvoiceLines.FirstOrDefault().Inventory.MarkupPrice,
                        Quantity = g.Count(),
                        Total = g.Sum(s => s.Subtotal)
                    }).ToList();

                // Calculate the appreciation
                var appreciation = salesDataList
                    .Select(s => s.Appreciation)
                    .Aggregate((a, b) => a + b);
                var totalAppreciation = salesDataList
                    .Select(s => s.Appreciation)
                    .Aggregate((a, b) => a + b);

                // Create the SalesReport object
                var salesReport = new SalesReport
                {
                    CashAmount = paymentTypeSummary.FirstOrDefault(pt => pt.PaymentType == "Cash")?.Amount ?? 0,
                    DebitAmount = paymentTypeSummary.FirstOrDefault(pt => pt.PaymentType == "Debit")?.Amount ?? 0,
                    CreditAmount = paymentTypeSummary.FirstOrDefault(pt => pt.PaymentType == "Credit")?.Amount ?? 0,
                    ChequeAmount = paymentTypeSummary.FirstOrDefault(pt => pt.PaymentType == "Cheque")?.Amount ?? 0,
                    Total = salesDataList.Sum(s => s.Subtotal),
                    //todo add tax
                };


                // If an employee is selected, create a SalesReportEmployee object for the sales report
                if (newReport.EmployeeId.HasValue)
                {
                    var salesReportEmployee = new SalesReportEmployee
                    {
                        EmployeeID = newReport.EmployeeId.Value,
                        Sales = employeeSummary.FirstOrDefault(e => e.EmployeeID == newReport.EmployeeId.Value)?.Sales ?? 0,
                        SalesReport = salesReport
                    };
                    salesReport.SalesReportEmployees.Add(salesReportEmployee);
                }
                else if (newReport.AllEmployees)
                {
                    foreach (var employeeSales in employeeSummary)
                    {
                        var salesReportEmployee = new SalesReportEmployee
                        {
                            EmployeeID = employeeSales.EmployeeID,
                            Sales = employeeSales.Sales,
                            SalesReport = salesReport
                        };
                        salesReport.SalesReportEmployees.Add(salesReportEmployee);
                    }
                }


                // Save the sales report to the database
                _context.SalesReports.Add(salesReport);

                //Add the sales report to the reports database
                Report reportToAdd = new Report
                {
                    Description = newReport.ReportName,
                    DateStart = newReport.StartDate,
                    DateEnd = newReport.EndDate,
                    Criteria = newReport.AllEmployees ? "All Employees" : $"{_context.Employees.FirstOrDefault(e => e.ID == newReport.EmployeeId)?.FirstName} {_context.Employees.FirstOrDefault(e => e.ID == newReport.EmployeeId)?.LastName}",
                    Type = 0,
                    DateCreated = DateTime.Now,
                    SalesReport = salesReport
                };


                _context.Reports.Add(reportToAdd);
                var savedSalesReports = await PaginatedList<SalesReport>.CreateAsync(_context.SalesReports, page ?? 1, pageSize ?? 5);



                var reportsVM = new ReportsVM
                {
                    SavedSalesReports = savedSalesReports,
                    PageSize = pageSize.Value,
                    PageIndex = page.Value,
                };

                await _context.SaveChangesAsync();

                return Json(new { success = true, report = reportToAdd, reportsVM = reportsVM });
            }
            catch (Exception ex)
            {
                // Log the exception
                // _logger.LogError(ex, "Error creating a new sales report");

                return Json(new { success = false, message = "An error occurred while saving the report. Exception Message: " + ex.Message });
            }
        }
        public async Task<IActionResult> GenerateSalesReportPDF(int id)
        {
            // Retrieve the report details from the database using the reportId parameter
            var report = await _context.Reports
                        .Include(r => r.SalesReport)
                        .ThenInclude(sr => sr.SalesReportEmployees)
                        .ThenInclude(sre => sre.Employee)
                        .FirstOrDefaultAsync(r => r.ID == id);
            // Check if the report is null and return a proper response
            if (report == null)
            {
                return NotFound($"Report with ID {id} not found.");
            }


            // Create the PDF document
            using (var memoryStream = new MemoryStream())
            {
                var writer = new PdfWriter(memoryStream);
                var pdf = new PdfDocument(writer);
                var document = new Document(pdf);

                // Set up the PDF formatting and styles
                var titleFontSize = 18f;
                var subtitleFontSize = 14f;
                var textFontSize = 12f;
                var titleFont = PdfFontFactory.CreateFont(iText.IO.Font.Constants.StandardFonts.HELVETICA_BOLD);
                var subtitleFont = PdfFontFactory.CreateFont(iText.IO.Font.Constants.StandardFonts.HELVETICA);
                var textFont = PdfFontFactory.CreateFont(iText.IO.Font.Constants.StandardFonts.HELVETICA);

                // Add the content (text, tables, etc.) to the PDF document
                document.Add(new Paragraph("Emma's Small Engine")
                    .SetFont(titleFont)
                    .SetFontSize(titleFontSize)
                    .SetTextAlignment(TextAlignment.CENTER));

                document.Add(new Paragraph("End of Day Report")
                    .SetFont(subtitleFont)
                    .SetFontSize(subtitleFontSize)
                    .SetTextAlignment(TextAlignment.CENTER));

                document.Add(new Paragraph($"Report Date: {report.DateCreated.ToString("MM/dd/yyyy")}")
                    .SetFont(textFont)
                    .SetFontSize(textFontSize)
                    .SetTextAlignment(TextAlignment.CENTER));

                // Payment Type Summary
                var paymentTypeSummaryTable = new Table(UnitValue.CreatePercentArray(new float[] { 1, 1, 1, 1, 1 }));
                paymentTypeSummaryTable.SetWidth(UnitValue.CreatePercentValue(100));
                paymentTypeSummaryTable.AddHeaderCell(new Cell().Add(new Paragraph("Payment Type").SetFont(subtitleFont).SetFontSize(subtitleFontSize)));
                paymentTypeSummaryTable.AddHeaderCell(new Cell().Add(new Paragraph("Amount").SetFont(subtitleFont).SetFontSize(subtitleFontSize)));
                paymentTypeSummaryTable.AddCell(new Cell().Add(new Paragraph("Cash").SetFont(textFont).SetFontSize(textFontSize)));
                paymentTypeSummaryTable.AddCell(new Cell().Add(new Paragraph(report.SalesReport.CashAmount.ToString("C")).SetFont(textFont).SetFontSize(textFontSize)));
                paymentTypeSummaryTable.AddCell(new Cell().Add(new Paragraph("Debit").SetFont(textFont).SetFontSize(textFontSize)));
                paymentTypeSummaryTable.AddCell(new Cell().Add(new Paragraph(report.SalesReport.DebitAmount.ToString("C")).SetFont(textFont).SetFontSize(textFontSize)));
                paymentTypeSummaryTable.AddCell(new Cell().Add(new Paragraph("Credit").SetFont(textFont).SetFontSize(textFontSize)));
                paymentTypeSummaryTable.AddCell(new Cell().Add(new Paragraph(report.SalesReport.CreditAmount.ToString("C")).SetFont(textFont).SetFontSize(textFontSize)));
                paymentTypeSummaryTable.AddCell(new Cell().Add(new Paragraph("Check").SetFont(textFont).SetFontSize(textFontSize)));
                paymentTypeSummaryTable.AddCell(new Cell().Add(new Paragraph(report.SalesReport.ChequeAmount.ToString("C")).SetFont(textFont).SetFontSize(textFontSize)));
                paymentTypeSummaryTable.AddCell(new Cell().Add(new Paragraph("Total").SetFont(textFont).SetFontSize(textFontSize)));
                paymentTypeSummaryTable.AddCell(new Cell().Add(new Paragraph(report.SalesReport.Total.ToString("C")).SetFont(textFont).SetFontSize(textFontSize)));

                document.Add(paymentTypeSummaryTable);

                // Tax Summary
                var taxSummaryTable = new Table(UnitValue.CreatePercentArray(new float[] { 1, 1, 1, 1, 1 }));
                taxSummaryTable.SetWidth(UnitValue.CreatePercentValue(100));
                taxSummaryTable.AddHeaderCell(new Cell().Add(new Paragraph("Tax Type").SetFont(subtitleFont).SetFontSize(subtitleFontSize)));
                taxSummaryTable.AddHeaderCell(new Cell().Add(new Paragraph("Amount").SetFont(subtitleFont).SetFontSize(subtitleFontSize)));
                //Sales Tax
                taxSummaryTable.AddCell(new Cell().Add(new Paragraph("Sales Tax").SetFont(textFont).SetFontSize(textFontSize)));
                taxSummaryTable.AddCell(new Cell().Add(new Paragraph(report.SalesReport.SalesTax.ToString("C")).SetFont(textFont).SetFontSize(textFontSize)));
                //Other Tax
                taxSummaryTable.AddCell(new Cell().Add(new Paragraph("Other Tax").SetFont(textFont).SetFontSize(textFontSize)));
                taxSummaryTable.AddCell(new Cell().Add(new Paragraph(report.SalesReport.OtherTax.ToString("C")).SetFont(textFont).SetFontSize(textFontSize)));
                //Total Tax
                taxSummaryTable.AddCell(new Cell().Add(new Paragraph("Total Tax").SetFont(textFont).SetFontSize(textFontSize)));
                taxSummaryTable.AddCell(new Cell().Add(new Paragraph(report.SalesReport.TotalTax.ToString("C")).SetFont(textFont).SetFontSize(textFontSize)));

                document.Add(taxSummaryTable);

                // Employee Summary (employees, total sales)
                var employeeSummaryTable = new Table(UnitValue.CreatePercentArray(new float[] { 1, 1, 1, 1, 1 }));
                employeeSummaryTable.SetWidth(UnitValue.CreatePercentValue(100));
                employeeSummaryTable.AddHeaderCell(new Cell().Add(new Paragraph("Employee").SetFont(subtitleFont).SetFontSize(subtitleFontSize)));
                employeeSummaryTable.AddHeaderCell(new Cell().Add(new Paragraph("Sales").SetFont(subtitleFont).SetFontSize(subtitleFontSize)));

                foreach (var employee in report.SalesReport.SalesReportEmployees)
                {
                    employeeSummaryTable.AddCell(new Cell().Add(new Paragraph(employee.Employee.FullName).SetFont(textFont).SetFontSize(textFontSize)));
                    employeeSummaryTable.AddCell(new Cell().Add(new Paragraph(employee.Sales.ToString("C")).SetFont(textFont).SetFontSize(textFontSize)));
                }

                employeeSummaryTable.AddCell(new Cell().Add(new Paragraph("Total").SetFont(textFont).SetFontSize(textFontSize)));
                employeeSummaryTable.AddCell(new Cell().Add(new Paragraph(report.SalesReport.Total.ToString("C")).SetFont(textFont).SetFontSize(textFontSize)));

                document.Add(employeeSummaryTable);

                // Sales Summary (items, total sales, quantity, price)
                var salesSummaryTable = new Table(UnitValue.CreatePercentArray(new float[] { 1, 1, 1, 1, 1 }));
                salesSummaryTable.SetWidth(UnitValue.CreatePercentValue(100));
                salesSummaryTable.AddHeaderCell(new Cell().Add(new Paragraph("Item").SetFont(subtitleFont).SetFontSize(subtitleFontSize)));
                salesSummaryTable.AddHeaderCell(new Cell().Add(new Paragraph("Quantity").SetFont(subtitleFont).SetFontSize(subtitleFontSize)));
                salesSummaryTable.AddHeaderCell(new Cell().Add(new Paragraph("Price").SetFont(subtitleFont).SetFontSize(subtitleFontSize)));
                salesSummaryTable.AddHeaderCell(new Cell().Add(new Paragraph("Sales").SetFont(subtitleFont).SetFontSize(subtitleFontSize)));

                foreach (var item in report.SalesReport.SalesReportInventories)
                {
                    salesSummaryTable.AddCell(new Cell().Add(new Paragraph(item.Inventory.Name).SetFont(textFont).SetFontSize(textFontSize)));
                    salesSummaryTable.AddCell(new Cell().Add(new Paragraph(item.Quantity.ToString()).SetFont(textFont).SetFontSize(textFontSize)));
                    salesSummaryTable.AddCell(new Cell().Add(new Paragraph(item.Inventory.MarkupPrice.ToString("C")).SetFont(textFont).SetFontSize(textFontSize)));
                    salesSummaryTable.AddCell(new Cell().Add(new Paragraph(item.Total.ToString("C")).SetFont(textFont).SetFontSize(textFontSize)));
                }

                document.Add(salesSummaryTable);

                // Appreciation (appreciation earned (2% sales), appreciation earned to date)
                var appreciationSummaryTable = new Table(UnitValue.CreatePercentArray(new float[] { 1, 1, 1, 1, 1 }));
                appreciationSummaryTable.SetWidth(UnitValue.CreatePercentValue(100));
                appreciationSummaryTable.AddHeaderCell(new Cell().Add(new Paragraph("Appreciation").SetFont(subtitleFont).SetFontSize(subtitleFontSize)));
                appreciationSummaryTable.AddHeaderCell(new Cell().Add(new Paragraph("Amount").SetFont(subtitleFont).SetFontSize(subtitleFontSize)));
                //Appreciation Earned
                appreciationSummaryTable.AddCell(new Cell().Add(new Paragraph("Appreciation Earned").SetFont(textFont).SetFontSize(textFontSize)));
                // Calculate appreciation earned (2% of sales)
                var appreciationEarned = report.SalesReport.Total * 0.02;
                appreciationSummaryTable.AddCell(new Cell().Add(new Paragraph(appreciationEarned.ToString("C")).SetFont(textFont).SetFontSize(textFontSize)));
                //Appreciation Earned to Date
                appreciationSummaryTable.AddCell(new Cell().Add(new Paragraph("Appreciation Earned to Date").SetFont(textFont).SetFontSize(textFontSize)));
                // Calculate appreciation earned to date (2% of sales + appreciation earned to date)
                // TODO: Get appreciation earned to date from database
                var appreciationEarnedToDate = appreciationEarned + 0;
                appreciationSummaryTable.AddCell(new Cell().Add(new Paragraph(appreciationEarnedToDate.ToString("C")).SetFont(textFont).SetFontSize(textFontSize)));

                document.Add(appreciationSummaryTable);

                // Footer (Report generated by {employee name} on {date})
                var footerTable = new Table(UnitValue.CreatePercentArray(new float[] { 1, 1, 1, 1, 1 }));
                footerTable.SetWidth(UnitValue.CreatePercentValue(100));
                footerTable.AddCell(new Cell().Add(new Paragraph($"Report generated by Emma Ham on {report.DateCreated}").SetFont(textFont).SetFontSize(textFontSize)));

                document.Add(footerTable);


                // Close the document
                document.Close();

                // Return the PDF as a byte array
                var pdfByteArray = memoryStream.ToArray();
                return File(pdfByteArray, "application/pdf", $"Report_{id}.pdf");
            }
        }
        #endregion

        #region Hourly Report

        [HttpPost]
        public async Task<IActionResult> CreateHourlyReport([FromForm] NewHourlyReport newReport, int? page, int? pageSize)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { success = false, message = "Invalid form data." });
            }

            try
            {
                // Check the input parameters for null values
                page ??= 1;
                pageSize ??= 5;
                var employeeData = _context.Employees
                    .Include(s => s.EmployeeLogins.Where(s => newReport.StartDate <= s.SignIn && s.SignIn <= newReport.EndDate))
                    .AsQueryable();

                if (!newReport.AllEmployees && newReport.EmployeeId.HasValue)
                {
                    employeeData = employeeData.Where(s => s.ID == newReport.EmployeeId.Value);
                }

                var empDataList = await employeeData.ToListAsync();

                await _context.SaveChangesAsync();
                //Add the hourly report to the reports database
                Report reportToAdd = new Report
                {
                    Description = newReport.ReportName,
                    DateStart = newReport.StartDate,
                    DateEnd = newReport.EndDate,
                    Criteria = newReport.AllEmployees ? "All Employees" : $"{_context.Employees.FirstOrDefault(e => e.ID == newReport.EmployeeId)?.FirstName} {_context.Employees.FirstOrDefault(e => e.ID == newReport.EmployeeId)?.LastName}",
                    Type = ReportType.Hourly,
                    DateCreated = DateTime.Now,
                    HourlyReport = new HourlyReport()
                    {
                        Employees = empDataList
                    }
            };


                _context.Reports.Add(reportToAdd);
                await _context.SaveChangesAsync();
                var savedHourlyReports = await PaginatedList<HourlyReport>.CreateAsync(_context.HourlyReports, page ?? 1, pageSize ?? 5);



                var reportsVM = new ReportsVM
                {
                    SavedHourlyReports = savedHourlyReports,
                    PageSize = pageSize.Value,
                    PageIndex = page.Value,
                };

                await _context.SaveChangesAsync();

                return Json(new { success = true, report = reportToAdd, reportsVM = reportsVM });
            }
            catch (Exception ex)
            {
                // Log the exception
                // _logger.LogError(ex, "Error creating a new sales report");

                return Json(new { success = false, message = "An error occurred while saving the report. Exception Message: " + ex.Message });
            }
        }

        #endregion

    }
}

