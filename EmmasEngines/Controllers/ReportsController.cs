﻿using EmmasEngines.Data;
using EmmasEngines.Models;
using EmmasEngines.Utilities;
using EmmasEngines.ViewModels;
using iText.Kernel.Colors;
using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Borders;
using iText.Layout.Element;
using iText.Layout.Properties;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Composition;
using System.Text.Json;
using System.Text.Json.Serialization;
using Table = iText.Layout.Element.Table;

namespace EmmasEngines.Controllers
{
    [Authorize(Roles = "Admin")]
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
            var COGSReports = await PaginatedList<COGSReport>.CreateAsync(_context.COGSReports.AsQueryable(), page ?? 1, pageSize ?? 5);
            var savedCOGSReports = await PaginatedList<Report>.CreateAsync(_context.Reports.Where(i => i.Type == ReportType.COGS), page ?? 1, pageSize ?? 5);


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
                SavedCOGSReports = COGSReports,
                COGSReportVM = new COGSReportVM
                {
                    SavedCOGSReports = savedCOGSReports,
                    Inventories = await _context.Inventories.ToListAsync(),
                    Invoices = await _context.Invoices.ToListAsync()
                },
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
                    .Where(s => newReport.StartDate <= s.Date && s.Date <= newReport.EndDate.Date.AddHours(23).AddMinutes(59).AddSeconds(59))
                    .AsQueryable();

                DateTime startDate = newReport.StartDate;
                DateTime endDate = newReport.EndDate;
                string sqlQuery = salesData.ToQueryString();

                if (newReport.EmployeeId.HasValue)
                {
                    salesData = salesData.Where(s => s.Employee.ID == newReport.EmployeeId.Value);
                }
                else
                {
                    newReport.AllEmployees = true;
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
                var taxSummary = new
                {
                    SalesTax = salesDataList.Sum(s => s.Subtotal * 0.13),
                    OtherTaxes = 0 // You can add other taxes here when needed
                };

                // Calculate the employee summary
                var employeeSummary = salesDataList.GroupBy(s => s.Employee.ID)
                    .Select(g => new
                    {
                        EmployeeID = g.Key,
                        Sales = g.Sum(s => s.Subtotal)
                    }).ToList();

                // Calculate the sales summary and add to SalesReportInventory



                // Calculate the appreciation
                var appreciation = salesDataList
                    .Select(s => s.Appreciation)
                    .Aggregate((a, b) => a + b);
                var totalAppreciation = salesDataList
                    .Select(s => s.Appreciation)
                    .Aggregate((a, b) => a + b);





                // Calculate the sales summary
                var salesSummary = salesDataList
                    .SelectMany(s => s.InvoiceLines)
                    .GroupBy(il => il.Inventory.UPC)
                    .Select(g => new
                    {
                        InventoryUPC = g.Key,
                        Quantity = g.Sum(il => il.Quantity),
                        Total = g.Sum(il => il.Quantity * il.SalePrice)
                    }).ToList();

                //Add the sales report to the reports database
                Report reportToAdd = new Report
                {
                    Description = newReport.ReportName,
                    DateStart = newReport.StartDate,
                    DateEnd = newReport.EndDate,
                    Criteria = newReport.AllEmployees ? "All Employees" : $"{_context.Employees.FirstOrDefault(e => e.ID == newReport.EmployeeId)?.FirstName} {_context.Employees.FirstOrDefault(e => e.ID == newReport.EmployeeId)?.LastName}",
                    Type = 0,
                    DateCreated = DateTime.Now
                };

                _context.Reports.Add(reportToAdd);
                await _context.SaveChangesAsync();

                // Create the SalesReport object
                var salesReport = new SalesReport
                {
                    ID = reportToAdd.ID,
                    CashAmount = paymentTypeSummary.FirstOrDefault(pt => pt.PaymentType == "Cash")?.Amount ?? 0,
                    DebitAmount = paymentTypeSummary.FirstOrDefault(pt => pt.PaymentType == "Debit")?.Amount ?? 0,
                    CreditAmount = paymentTypeSummary.FirstOrDefault(pt => pt.PaymentType == "Credit")?.Amount ?? 0,
                    ChequeAmount = paymentTypeSummary.FirstOrDefault(pt => pt.PaymentType == "Cheque")?.Amount ?? 0,
                    Total = salesDataList.Sum(s => s.Subtotal),
                    SalesTax = taxSummary.SalesTax,
                    OtherTax = taxSummary.OtherTaxes,
                    TotalTax = taxSummary.SalesTax + taxSummary.OtherTaxes
                };

                // Add the sales summary to SalesReportInventory
                foreach (var summary in salesSummary)
                {
                    var inventory = await _context.Inventories.FirstOrDefaultAsync(i => i.UPC == summary.InventoryUPC);

                    var salesReportInventory = new SalesReportInventory
                    {
                        InventoryUPC = summary.InventoryUPC,
                        Inventory = inventory,
                        Quantity = summary.Quantity,
                        Total = summary.Total,
                        SalesReportID = salesReport.ID,
                        SalesReport = salesReport
                    };
                    salesReport.SalesReportInventories.Add(salesReportInventory);
                }

                // Save the SalesReportInventory objects to the database
                await _context.SaveChangesAsync();


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
                await _context.SaveChangesAsync();

                var savedSalesReports = await PaginatedList<SalesReport>.CreateAsync(_context.SalesReports, page ?? 1, pageSize ?? 5);
                var reportsVM = new ReportsVM
                {
                    SavedSalesReports = savedSalesReports,
                    PageSize = pageSize.Value,
                    PageIndex = page.Value,
                };

                await _context.SaveChangesAsync();
                var options = new JsonSerializerOptions
                {
                    ReferenceHandler = ReferenceHandler.Preserve
                };


                return new JsonResult(new { success = true, report = reportToAdd }, options);
            }
            catch (DbUpdateException ex)
            {
                var message = "Database update error: " + ex.InnerException?.Message ?? ex.Message;

                // Log the exception
                //_logger.LogError(ex, message);

                return Json(new { success = false, message = message });
            }
            catch (Exception ex)
            {
                // Log the exception
                // _logger.LogError(ex, "Error creating a new sales report");

                return Json(new { success = false, message = $"An error occurred while saving the report. Exception Message: {ex.Message}", stackTrace = ex.StackTrace });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var report = await _context.Reports.FindAsync(id);

                if (report == null)
                {
                    return NotFound();
                }

                // Find the corresponding SalesReport entity
                var salesReport = await _context.SalesReports.FirstOrDefaultAsync(sr => sr.ID == id);

                if (salesReport != null)
                {
                    // Delete all the SalesReportInventories associated with the SalesReport
                    _context.SalesReportInventories.RemoveRange(salesReport.SalesReportInventories);

                    // Delete all the SalesReportEmployees associated with the SalesReport
                    _context.SalesReportEmployees.RemoveRange(salesReport.SalesReportEmployees);

                    // Delete the SalesReport entity
                    _context.SalesReports.Remove(salesReport);
                }

                // Delete the Report entity
                _context.Reports.Remove(report);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException ex)
            {
                var message = "Database update error: " + ex.InnerException?.Message ?? ex.Message;
                //_logger.LogError(ex, message);

                return Json(new { success = false, message = message });
            }
            catch (Exception ex)
            {
                // _logger.LogError(ex, "Error deleting the report");

                return Json(new { success = false, message = $"An error occurred while deleting the report. Exception Message: {ex.Message}", stackTrace = ex.StackTrace });
            }
        }


        public async Task<IActionResult> SalesReportDetails(int id)
        {
            var salesReport = await _context.SalesReports.Include(s => s.SalesReportEmployees)
                                                         .ThenInclude(e => e.Employee)
                                                         .Include(s => s.SalesReportInventories)
                                                         .ThenInclude(i => i.Inventory)
                                                         .ThenInclude(ie => ie.Prices)
                                                         .FirstOrDefaultAsync(s => s.ID == id);

            if (salesReport == null)
            {
                return NotFound();
            }

            // Calculate appreciation earned and appreciation earned to date
            double appreciationEarned = CalculateAppreciation(salesReport);
            double appreciationEarnedToDate = CalculateAppreciationToDate(salesReport);

            var viewModel = new SalesReportDetailsVM
            {
                SalesReport = salesReport,
                SalesReportEmployees = salesReport.SalesReportEmployees,
                SalesReportInventories = salesReport.SalesReportInventories,
                AppreciationEarned = appreciationEarned,
                AppreciationEarnedToDate = appreciationEarnedToDate
            };

            return View(viewModel);
        }

        private double CalculateAppreciation(SalesReport salesReport)
        {
            // calculate appreciation earned in selected date range
            double appr = 0;
            appr = salesReport.SalesReportInventories
                    .Select(i => (i.Inventory == null || i.Inventory.Prices.FirstOrDefault() == null) ? 0 : i.Inventory.Prices.FirstOrDefault().PurchasedCost * i.Quantity)
                    .Sum() * 0.02;
            return appr;
        }

        private double CalculateAppreciationToDate(SalesReport salesReport)
        {
            // calculate appreciation earned to date for all sales reports
            double apprToDate = 0;
            apprToDate = _context.SalesReports.Include(s => s.SalesReportInventories)
                                              .ThenInclude(i => i.Inventory)
                                              .ThenInclude(ie => ie.Prices)
                                              .Sum(s => s.SalesReportInventories
                                                        .Select(i => (i.Inventory == null || i.Inventory.Prices.FirstOrDefault() == null) ? 0 : i.Inventory.Prices.FirstOrDefault().PurchasedCost * i.Quantity)
                                                        .Sum()) * 0.02;
            return apprToDate;
        }

        public async Task<IActionResult> GenerateSalesReportPDF(int id)
        {
            // Retrieve the report details from the database using the reportId parameter
            var report = await _context.Reports
                        .Include(r => r.SalesReport)
                        .ThenInclude(sr => sr.SalesReportEmployees)
                        .ThenInclude(sre => sre.Employee)
                        .Include(r => r.SalesReport)
                        .ThenInclude(sr => sr.SalesReportInventories)
                        .ThenInclude(sri => sri.Inventory)
                        .ThenInclude(i => i.Prices)
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

                // Set margins
                document.SetMargins(36, 36, 36, 36);

                // Add the content (text, tables, etc.) to the PDF document
                document.Add(new Paragraph("Emma's Small Engine")
                    .SetFont(titleFont)
                    .SetFontSize(titleFontSize)
                    .SetTextAlignment(TextAlignment.CENTER)
                    .SetMarginBottom(10));

                document.Add(new Paragraph("End of Day Report")
                    .SetFont(subtitleFont)
                    .SetFontSize(subtitleFontSize)
                    .SetTextAlignment(TextAlignment.CENTER)
                    .SetMarginBottom(10));

                document.Add(new Paragraph($"Report Date: {report.DateCreated.ToString("MM/dd/yyyy")}")
                    .SetFont(textFont)
                    .SetFontSize(textFontSize)
                    .SetTextAlignment(TextAlignment.CENTER)
                    .SetMarginBottom(20));

                // Payment Type Summary
                var paymentTypeSummaryTable = new Table(UnitValue.CreatePercentArray(new float[] { 1, 1, 1, 1, 1 }));
                paymentTypeSummaryTable.SetWidth(UnitValue.CreatePercentValue(100));

                // Added header row with no borders
                var headerRow = new Cell(1, 5)
                    .SetBorder(Border.NO_BORDER)
                    .SetBackgroundColor(ColorConstants.LIGHT_GRAY)
                    .Add(new Paragraph("Payment Type Summary")
                        .SetFont(subtitleFont)
                        .SetFontSize(subtitleFontSize)
                        .SetTextAlignment(TextAlignment.CENTER));

                paymentTypeSummaryTable.AddHeaderCell(headerRow);
                paymentTypeSummaryTable.AddCell(CreateTableCell("Payment Type", subtitleFont, subtitleFontSize));
                paymentTypeSummaryTable.AddCell(CreateTableCell("Amount", subtitleFont, subtitleFontSize));
                paymentTypeSummaryTable.AddCell(new Cell(1, 3).SetBorder(Border.NO_BORDER)); // Added an empty cell to complete the row

                var paymentTypes = new[] { "Cash", "Debit", "Credit", "Cheque", "Total" };
                var paymentAmounts = new[]
                {
                    report.SalesReport.CashAmount,
                    report.SalesReport.DebitAmount,
                    report.SalesReport.CreditAmount,
                    report.SalesReport.ChequeAmount,
                    report.SalesReport.Total
                };

                for (int i = 0; i < paymentTypes.Length; i++)
                {
                    paymentTypeSummaryTable.StartNewRow(); // Start a new row for each data set
                    paymentTypeSummaryTable.AddCell(CreateTableCell(paymentTypes[i], textFont, textFontSize));
                    paymentTypeSummaryTable.AddCell(CreateTableCell(paymentAmounts[i].ToString("C"), textFont, textFontSize));
                }

                document.Add(paymentTypeSummaryTable.SetMarginBottom(20));

                // Tax Summary
                var taxSummaryTable = new Table(UnitValue.CreatePercentArray(new float[] { 1, 1, 1, 1, 1 }));
                taxSummaryTable.SetWidth(UnitValue.CreatePercentValue(100));

                // Added header row with no borders
                var taxHeaderRow = new Cell(1, 5)
                .SetBorder(Border.NO_BORDER)
                .SetBackgroundColor(ColorConstants.LIGHT_GRAY)
                .Add(new Paragraph("Tax Summary")
                .SetFont(subtitleFont)
                .SetFontSize(subtitleFontSize)
                .SetTextAlignment(TextAlignment.CENTER));

                taxSummaryTable.AddHeaderCell(taxHeaderRow);
                taxSummaryTable.AddCell(CreateTableCell("Tax Type", subtitleFont, subtitleFontSize));
                taxSummaryTable.AddCell(CreateTableCell("Amount", subtitleFont, subtitleFontSize));
                taxSummaryTable.AddCell(new Cell(1, 3).SetBorder(Border.NO_BORDER)); // Added an empty cell to complete the row

                var taxTypes = new[] { "Sales Tax", "Other Tax", "Total Tax" };
                var taxAmounts = new[]
                {
                    report.SalesReport.SalesTax,
                    report.SalesReport.OtherTax,
                    report.SalesReport.TotalTax
                };

                for (int i = 0; i < taxTypes.Length; i++)
                {
                    taxSummaryTable.StartNewRow(); // Start a new row for each data set
                    taxSummaryTable.AddCell(CreateTableCell(taxTypes[i], textFont, textFontSize));
                    taxSummaryTable.AddCell(CreateTableCell(taxAmounts[i].ToString("C"), textFont, textFontSize));
                }

                document.Add(taxSummaryTable.SetMarginBottom(20));

                // Employee Summary (employees, total sales)
                var employeeSummaryTable = new Table(UnitValue.CreatePercentArray(new float[] { 1, 1, 1, 1, 1 }));
                employeeSummaryTable.SetWidth(UnitValue.CreatePercentValue(100));

                // Added header row with no borders
                var employeeHeaderRow = new Cell(1, 5)
                    .SetBorder(Border.NO_BORDER)
                    .SetBackgroundColor(ColorConstants.LIGHT_GRAY)
                    .Add(new Paragraph("Employee Summary")
                        .SetFont(subtitleFont)
                        .SetFontSize(subtitleFontSize)
                        .SetTextAlignment(TextAlignment.CENTER));

                employeeSummaryTable.AddHeaderCell(employeeHeaderRow);
                employeeSummaryTable.AddCell(CreateTableCell("Employee", subtitleFont, subtitleFontSize));
                employeeSummaryTable.AddCell(CreateTableCell("Sales", subtitleFont, subtitleFontSize));
                taxSummaryTable.AddCell(new Cell(1, 3).SetBorder(Border.NO_BORDER)); // Added an empty cell to complete the row

                double total = 0;

                foreach (var employee in report.SalesReport.SalesReportEmployees)
                {
                    total += employee.Sales;
                    employeeSummaryTable.StartNewRow(); // Start a new row for each data set
                    employeeSummaryTable.AddCell(CreateTableCell(employee.Employee.FullName, textFont, textFontSize));
                    employeeSummaryTable.AddCell(CreateTableCell(employee.Sales.ToString("C"), textFont, textFontSize));
                }
                employeeSummaryTable.StartNewRow();
                employeeSummaryTable.AddCell(CreateTableCell("Total", textFont, textFontSize));
                employeeSummaryTable.AddCell(CreateTableCell(total.ToString("C"), textFont, textFontSize));

                document.Add(employeeSummaryTable.SetMarginBottom(20));

                // Sales Summary (items, total sales, quantity, price)
                var salesSummaryTable = new Table(UnitValue.CreatePercentArray(new float[] { 1, 1, 1, 1, 1 }));
                salesSummaryTable.SetWidth(UnitValue.CreatePercentValue(100));

                // Added header row with no borders
                var salesHeaderRow = new Cell(1, 5)
                    .SetBorder(Border.NO_BORDER)
                    .SetBackgroundColor(ColorConstants.LIGHT_GRAY)
                    .Add(new Paragraph("Sales Summary")
                        .SetFont(subtitleFont)
                        .SetFontSize(subtitleFontSize)
                        .SetTextAlignment(TextAlignment.CENTER));

                salesSummaryTable.AddHeaderCell(salesHeaderRow);
                salesSummaryTable.AddCell(CreateTableCell("Item", subtitleFont, subtitleFontSize));
                salesSummaryTable.AddCell(CreateTableCell("Quantity", subtitleFont, subtitleFontSize));
                salesSummaryTable.AddCell(CreateTableCell("Price", subtitleFont, subtitleFontSize));
                salesSummaryTable.AddCell(CreateTableCell("Total Sales", subtitleFont, subtitleFontSize));
                salesSummaryTable.AddCell(new Cell(1, 1).SetBorder(Border.NO_BORDER)); // Added an empty cell to complete the row

                foreach (var item in report.SalesReport.SalesReportInventories)
                {
                    salesSummaryTable.StartNewRow(); // Start a new row for each data set
                    salesSummaryTable.AddCell(CreateTableCell(item.Inventory.Name, textFont, textFontSize));
                    salesSummaryTable.AddCell(CreateTableCell(item.Quantity.ToString(), textFont, textFontSize));
                    salesSummaryTable.AddCell(CreateTableCell(item.Inventory.MarkupPrice.ToString("C"), textFont, textFontSize));
                    //sales total = inventory markup price * quantity
                    salesSummaryTable.AddCell(CreateTableCell((item.Inventory.MarkupPrice * item.Quantity).ToString("C"), textFont, textFontSize));
                }

                document.Add(salesSummaryTable.SetMarginBottom(20));

                // Appreciation (appreciation earned (2% sales), appreciation earned to date)
                var appreciationSummaryTable = new Table(UnitValue.CreatePercentArray(new float[] { 1, 1, 1, 1, 1 }));
                appreciationSummaryTable.SetWidth(UnitValue.CreatePercentValue(100));

                // Added header row with no borders
                var appreciationHeaderRow = new Cell(1, 5)
                    .SetBorder(Border.NO_BORDER)
                    .SetBackgroundColor(ColorConstants.LIGHT_GRAY)
                    .Add(new Paragraph("Appreciation Summary")
                        .SetFont(subtitleFont)
                        .SetFontSize(subtitleFontSize)
                        .SetTextAlignment(TextAlignment.CENTER));

                appreciationSummaryTable.AddHeaderCell(appreciationHeaderRow);
                appreciationSummaryTable.AddCell(CreateTableCell("Appreciation", subtitleFont, subtitleFontSize));
                appreciationSummaryTable.AddCell(CreateTableCell("Amount", subtitleFont, subtitleFontSize));
                taxSummaryTable.AddCell(new Cell(1, 3).SetBorder(Border.NO_BORDER)); // Added an empty cell to complete the row

                // Appreciation Earned
                appreciationSummaryTable.StartNewRow();
                appreciationSummaryTable.AddCell(CreateTableCell("Appreciation Earned", textFont, textFontSize));
                var appreciationEarned = report.SalesReport.Total * 0.02;
                appreciationSummaryTable.AddCell(CreateTableCell(appreciationEarned.ToString("C"), textFont, textFontSize));

                // Appreciation Earned to Date
                appreciationSummaryTable.StartNewRow();
                appreciationSummaryTable.AddCell(CreateTableCell("Appreciation Earned to Date", textFont, textFontSize));
                // TODO: Get appreciation earned to date from the database
                var appreciationEarnedToDate = appreciationEarned + 0;
                appreciationSummaryTable.AddCell(CreateTableCell(appreciationEarnedToDate.ToString("C"), textFont, textFontSize));

                document.Add(appreciationSummaryTable.SetMarginBottom(20));

                // Footer (Report generated by {employee name} on {date})
                var footerTable = new Table(UnitValue.CreatePercentArray(new float[] { 1, 1, 1, 1, 1 }));
                footerTable.SetWidth(UnitValue.CreatePercentValue(100));
                footerTable.SetBorder(Border.NO_BORDER);
                footerTable.AddCell(CreateTableCell($"Report generated by Emma Ham on {report.DateCreated}", textFont, textFontSize));

                document.Add(footerTable);

                // Close the document
                document.Close();

                // Return the PDF as a byte array
                var pdfByteArray = memoryStream.ToArray();
                return File(pdfByteArray, "application/pdf", $"Report_{id}.pdf");
            }
        }

        private Cell CreateTableCell(string content, PdfFont font, float fontSize)
        {
            return new Cell().Add(new Paragraph(content).SetFont(font).SetFontSize(fontSize));
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

                string selectedEmployee = "";

                if (newReport.EmployeeId != null)
                {
                    employeeData = employeeData.Where(s => s.ID == newReport.EmployeeId);
                    selectedEmployee = _context.Employees.Where(e => e.ID == newReport.EmployeeId).Select(e => e.FullName).FirstOrDefault();
                    newReport.AllEmployees = false;
                }
                else
                {
                    newReport.AllEmployees = true;
                }

                var empDataList = await employeeData.ToListAsync();

                

                //Add the hourly report to the reports database
                Report reportToAdd = new Report
                {
                    Description = newReport.ReportName,
                    DateStart = newReport.StartDate,
                    DateEnd = newReport.EndDate,
                    Criteria = newReport.AllEmployees ? "All Employees" : selectedEmployee,
                    Type = ReportType.Hourly,
                    DateCreated = DateTime.Now
                };
                _context.Reports.Add(reportToAdd);
                await _context.SaveChangesAsync();

                var hourlyReport = new HourlyReport
                {
                    ID = reportToAdd.ID,
                    Employees = empDataList
                };


                _context.HourlyReports.Add(hourlyReport);
                await _context.SaveChangesAsync();

                var savedHourlyReports = await PaginatedList<HourlyReport>.CreateAsync(_context.HourlyReports, page ?? 1, pageSize ?? 5);

                var reportsVM = new ReportsVM
                {
                    SavedHourlyReports = savedHourlyReports,
                    PageSize = pageSize.Value,
                    PageIndex = page.Value,
                };

                await _context.SaveChangesAsync();

                var options = new JsonSerializerOptions
                {
                    ReferenceHandler = ReferenceHandler.Preserve
                };

                return new JsonResult(new { success = true, report = reportToAdd }, options);
            }
            catch (DbUpdateException ex)
            {
                var message = "Database update error: " + ex.InnerException?.Message ?? ex.Message;

                return Json(new { success = false, message = message });
            }
            catch (Exception ex)
            {

                return Json(new { success = false, message = $"An error occurred while saving the report. Exception Message: {ex.Message}", stackTrace = ex.StackTrace });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteHourlyReport(int id)
        {
            try
            {
                var report = await _context.Reports.FindAsync(id);

                if (report == null)
                {
                    return NotFound();
                }

                // Find the corresponding SalesReport entity
                var hourlyReport = await _context.HourlyReports.FirstOrDefaultAsync(hr => hr.ID == id);

                if (hourlyReport != null)
                {
                    // Delete the HourlyReport entity
                    _context.HourlyReports.Remove(hourlyReport);
                }

                // Delete the Report entity
                _context.Reports.Remove(report);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException ex)
            {
                var message = "Database update error: " + ex.InnerException?.Message ?? ex.Message;
                //_logger.LogError(ex, message);

                return Json(new { success = false, message = message });
            }
            catch (Exception ex)
            {
                // _logger.LogError(ex, "Error deleting the report");

                return Json(new { success = false, message = $"An error occurred while deleting the report. Exception Message: {ex.Message}", stackTrace = ex.StackTrace });
            }
        }

        public async Task<IActionResult> GenerateHourlyReportPDF(int id)
        {
            // Retrieve the report details from the database using the reportId parameter
            var report = await _context.Reports
                        .Where(r => r.Type == ReportType.Hourly)
                        .Include(r => r.HourlyReport)
                        .ThenInclude(hr => hr.Employees)
                        .ThenInclude(hre => hre.EmployeeLogins)
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

                // Set margins
                document.SetMargins(36, 36, 36, 36);

                // Add the content (text, tables, etc.) to the PDF document
                document.Add(new Paragraph("Emma's Small Engine")
                    .SetFont(titleFont)
                    .SetFontSize(titleFontSize)
                    .SetTextAlignment(TextAlignment.CENTER)
                    .SetMarginBottom(10));

                document.Add(new Paragraph("Hourly Report")
                    .SetFont(subtitleFont)
                    .SetFontSize(subtitleFontSize)
                    .SetTextAlignment(TextAlignment.CENTER)
                    .SetMarginBottom(10));

                document.Add(new Paragraph($"Report Date: {report.DateCreated.ToString("MM/dd/yyyy")}")
                    .SetFont(textFont)
                    .SetFontSize(textFontSize)
                    .SetTextAlignment(TextAlignment.CENTER)
                    .SetMarginBottom(20));


                // Employee Login Summary
                var employeeSummaryTable = new Table(UnitValue.CreatePercentArray(new float[] { 1, 1, 1, 1 }));
                employeeSummaryTable.SetWidth(UnitValue.CreatePercentValue(100));

                // Added header row with no borders
                var employeeHeaderRow = new Cell(1, 5)
                    .SetBorder(Border.NO_BORDER)
                    .SetBackgroundColor(ColorConstants.LIGHT_GRAY)
                    .Add(new Paragraph("Employee Summary")
                        .SetFont(subtitleFont)
                        .SetFontSize(subtitleFontSize)
                        .SetTextAlignment(TextAlignment.CENTER));

                employeeSummaryTable.AddHeaderCell(employeeHeaderRow);
                employeeSummaryTable.AddCell(CreateTableCell("ID", subtitleFont, subtitleFontSize));
                employeeSummaryTable.AddCell(CreateTableCell("Name", subtitleFont, subtitleFontSize));
                employeeSummaryTable.AddCell(CreateTableCell("Date(s)", subtitleFont, subtitleFontSize));
                employeeSummaryTable.AddCell(CreateTableCell("Billable Hours", subtitleFont, subtitleFontSize));

                double totalHours = 0.00;

                foreach (var employee in report.HourlyReport.Employees)
                {
                    List<string> dates = await _context.EmployeeLogins.Where(l => l.SignIn >= report.DateStart && l.SignIn <= report.DateEnd && l.EmployeeID == employee.ID).Select(l => l.SignIn.ToShortDateString()).Distinct().ToListAsync();
                    string datestring = String.Join("\n", dates);
                    //double hours = _context.EmployeeLogins.Where(l => l.SignIn >= report.DateStart && l.SignIn <= report.DateEnd && l.EmployeeID == employee.ID).Select(l => (l.SignOut - l.SignIn).TotalDays).Sum();
                    List<EmployeeLogin> loginsSummary = _context.Employees.Where(e => e.ID == employee.ID).SelectMany(e => e.EmployeeLogins).ToList();
                    double hours = (loginsSummary.Where(l => l.SignIn >= report.DateStart && l.SignIn <= report.DateEnd).Select(l => (l.SignOut - l.SignIn).TotalHours)).Sum();
                    totalHours += hours;
                    employeeSummaryTable.StartNewRow(); // Start a new row for each data set
                    employeeSummaryTable.AddCell(CreateTableCell(employee.ID.ToString(), textFont, textFontSize));
                    employeeSummaryTable.AddCell(CreateTableCell(employee.FullName, textFont, textFontSize));
                    employeeSummaryTable.AddCell(CreateTableCell(datestring, textFont, textFontSize));
                    employeeSummaryTable.AddCell(CreateTableCell(hours.ToString("F"), textFont, textFontSize));
                }

                employeeSummaryTable.StartNewRow();
                employeeSummaryTable.AddCell(new Cell(1, 2).SetBorder(Border.NO_BORDER));
                employeeSummaryTable.AddCell(CreateTableCell("Total", textFont, textFontSize));
                employeeSummaryTable.AddCell(CreateTableCell(totalHours.ToString("F"), textFont, textFontSize));

                document.Add(employeeSummaryTable.SetMarginBottom(20));

                // Employee Logins
                var employeeLoginTable = new Table(UnitValue.CreatePercentArray(new float[] { 1, 1, 1, 1, 1,1 }));
                employeeLoginTable.SetWidth(UnitValue.CreatePercentValue(100));

                // Added header row with no borders
                var employeeLoginHeaderRow = new Cell(1, 6)
                    .SetBorder(Border.NO_BORDER)
                    .SetBackgroundColor(ColorConstants.LIGHT_GRAY)
                    .Add(new Paragraph("Employee Logins")
                        .SetFont(subtitleFont)
                        .SetFontSize(subtitleFontSize)
                        .SetTextAlignment(TextAlignment.CENTER));

                employeeLoginTable.AddHeaderCell(employeeLoginHeaderRow);
                employeeLoginTable.AddCell(CreateTableCell("ID", subtitleFont, subtitleFontSize));
                employeeLoginTable.AddCell(CreateTableCell("Name", subtitleFont, subtitleFontSize));
                employeeLoginTable.AddCell(CreateTableCell("Date", subtitleFont, subtitleFontSize));
                employeeLoginTable.AddCell(CreateTableCell("Billable Hours", subtitleFont, subtitleFontSize));
                employeeLoginTable.AddCell(CreateTableCell("Sign In", subtitleFont, subtitleFontSize));
                employeeLoginTable.AddCell(CreateTableCell("Sign Out", subtitleFont, subtitleFontSize));


                foreach (var employee in report.HourlyReport.Employees.OrderBy(e => e.FullName))
                {
                    List<EmployeeLogin> logins = await _context.EmployeeLogins.Where(l => l.SignIn >= report.DateStart && l.SignIn <= report.DateEnd && l.EmployeeID == employee.ID).ToListAsync();
                    foreach (var login in logins)
                    {
                        employeeLoginTable.StartNewRow(); // Start a new row for each data set
                        employeeLoginTable.AddCell(CreateTableCell(employee.ID.ToString(), textFont, textFontSize));
                        employeeLoginTable.AddCell(CreateTableCell(employee.FullName, textFont, textFontSize));
                        employeeLoginTable.AddCell(CreateTableCell(login.SignIn.ToShortDateString(), textFont, textFontSize));
                        employeeLoginTable.AddCell(CreateTableCell((login.SignOut - login.SignIn).TotalHours.ToString(), textFont, textFontSize));
                        employeeLoginTable.AddCell(CreateTableCell(login.SignIn.ToShortTimeString(), textFont, textFontSize));
                        employeeLoginTable.AddCell(CreateTableCell(login.SignOut.ToShortTimeString(), textFont, textFontSize));
                    }
                }

                document.Add(employeeLoginTable.SetMarginBottom(20));



                // Footer (Report generated by {employee name} on {date})
                var footerTable = new Table(UnitValue.CreatePercentArray(new float[] { 1, 1, 1, 1, 1 }));
                footerTable.SetWidth(UnitValue.CreatePercentValue(100));
                footerTable.SetBorder(Border.NO_BORDER);
                footerTable.AddCell(CreateTableCell($"Report generated by Emma Ham on {report.DateCreated}", textFont, textFontSize));

                document.Add(footerTable);

                // Close the document
                document.Close();

                // Return the PDF as a byte array
                var pdfByteArray = memoryStream.ToArray();
                return File(pdfByteArray, "application/pdf", $"Report_{id}.pdf");
            }
        }


        public async Task<IActionResult> HourlyReportDetails(int id)
        {
            var report = await _context.Reports.Where(r => r.Type == ReportType.Hourly)
                .Include(s => s.HourlyReport)
                .ThenInclude(s => s.Employees)
                .ThenInclude(e => e.EmployeeLogins)
                .FirstOrDefaultAsync(s => s.ID == id);

            if (report == null)
            {
                return NotFound();
            }


            var viewModel = new HourlyReportDetailsVM();
            viewModel.HourlyReport = report.HourlyReport;
            viewModel.Employees = report.HourlyReport.Employees;
            viewModel.Start = report.DateStart;
            viewModel.End = report.DateEnd;

            return View(viewModel);
        }

        #endregion

        #region COGS Report

        //COGS Report
        [HttpPost]
        public async Task<IActionResult> CreateCOGSReport([FromForm] NewCOGSReport newReport, int? page, int? pageSize)
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

                //Select data from Inventory and Invoices within dates
                var inventoryData = _context.Inventories
                    .Include(invent => invent.Prices.Where(invent => newReport.StartDate <= invent.PurchasedDate && invent.PurchasedDate <= newReport.EndDate))
                    .AsQueryable();

                var invoiceData = _context.Invoices
                    .Include(invo => invo.InvoiceLines.Where(invo => newReport.StartDate <= invo.Invoice.Date && invo.Invoice.Date <= newReport.EndDate))
                    .AsQueryable();

                //filter data from select with required products
                if (!newReport.AllInventory && newReport.InventoryId.HasValue)
                {
                    inventoryData = inventoryData.Where(invent => invent.UPC == newReport.InventoryId.Value.ToString());
                    invoiceData = invoiceData.Where(invo => invo.InvoiceLines.Any(involine => involine.InventoryUPC == newReport.InventoryId.Value.ToString()));
                }

                var inventoryDataList = await inventoryData.ToListAsync();
                var invoiceDataList = await invoiceData.ToListAsync();


                await _context.SaveChangesAsync();
                //Add the hourly report to the reports database
                Report reportToAdd = new Report
                {
                    Description = newReport.ReportName,
                    DateStart = newReport.StartDate,
                    DateEnd = newReport.EndDate,
                    Criteria = newReport.AllInventory ? "All Inventory" : $"{_context.Inventories.FirstOrDefault(inven => inven.UPC == newReport.InventoryId.Value.ToString())?.Name}",
                    Type = ReportType.COGS,
                    DateCreated = DateTime.Now,
                    COGSReport = new COGSReport()
                    {
                        Inventories = inventoryDataList,
                        Invoices = invoiceDataList
                    }
                };


                _context.Reports.Add(reportToAdd);
                await _context.SaveChangesAsync();
                var savedCOGSReports = await PaginatedList<COGSReport>.CreateAsync(_context.COGSReports, page ?? 1, pageSize ?? 5);



                var reportsVM = new ReportsVM
                {
                    SavedCOGSReports = savedCOGSReports,
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

        public async Task<IActionResult> GenerateCOGSReportPDF(int id)
        {
            // Retrieve the report details from the database using the reportId parameter
            var report = await _context.Reports
                        .Where(l => l.Type == ReportType.COGS)
                        .Include(m => m.COGSReport)
                        .ThenInclude(n => n.Inventories)
                        .ThenInclude(o => o.Prices)
                        .Include(p => p.COGSReport)
                        .ThenInclude(q => q.Invoices)
                        .ThenInclude(r => r.InvoiceLines)
                        .FirstOrDefaultAsync(s => s.ID == id);
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

                // Set margins
                document.SetMargins(36, 36, 36, 36);

                // Add the content (text, tables, etc.) to the PDF document
                document.Add(new Paragraph("Emma's Small Engine")
                    .SetFont(titleFont)
                    .SetFontSize(titleFontSize)
                    .SetTextAlignment(TextAlignment.CENTER)
                    .SetMarginBottom(10));

                document.Add(new Paragraph("COGS Report")
                    .SetFont(subtitleFont)
                    .SetFontSize(subtitleFontSize)
                    .SetTextAlignment(TextAlignment.CENTER)
                    .SetMarginBottom(10));

                document.Add(new Paragraph($"Report Date: {report.DateCreated.ToString("MM/dd/yyyy")}")
                    .SetFont(textFont)
                    .SetFontSize(textFontSize)
                    .SetTextAlignment(TextAlignment.CENTER)
                    .SetMarginBottom(20));


                // COGS
                var COGSSummaryTable = new Table(UnitValue.CreatePercentArray(new float[] { 1, 1, 1, 1 }));
                COGSSummaryTable.SetWidth(UnitValue.CreatePercentValue(100));

                // Added header row with no borders
                var COGSHeaderRow = new Cell(1, 5)
                    .SetBorder(Border.NO_BORDER)
                    .SetBackgroundColor(ColorConstants.LIGHT_GRAY)
                    .Add(new Paragraph("Costs Report")
                        .SetFont(subtitleFont)
                        .SetFontSize(subtitleFontSize)
                        .SetTextAlignment(TextAlignment.CENTER));

                COGSSummaryTable.AddHeaderCell(COGSHeaderRow);
                COGSSummaryTable.AddCell(CreateTableCell("Start Cost", subtitleFont, subtitleFontSize));
                COGSSummaryTable.AddCell(CreateTableCell("Purchased Cost", subtitleFont, subtitleFontSize));
                COGSSummaryTable.AddCell(CreateTableCell("End Cost", subtitleFont, subtitleFontSize));
                COGSSummaryTable.AddCell(CreateTableCell("COGS", subtitleFont, subtitleFontSize));
                COGSSummaryTable.AddCell(CreateTableCell("Sales Revenue", subtitleFont, subtitleFontSize));
                COGSSummaryTable.AddCell(CreateTableCell("Gross Profit", subtitleFont, subtitleFontSize));
                COGSSummaryTable.AddCell(CreateTableCell("Profit Margin", subtitleFont, subtitleFontSize));

                foreach (var inventory in report.COGSReport.Inventories.OrderBy(e => e.Name))
                {
                    List<string> dates = await _context.Prices.Where(l => l.PurchasedDate >= report.DateStart && l.PurchasedDate <= report.DateEnd && l.InventoryUPC == inventory.UPC).Select(l => l.PurchasedDate.ToShortDateString()).Distinct().ToListAsync();
                    string datestring = String.Join(",", dates);
                    List<Price> PriceSummary = _context.Prices.Where(e => e.ID == inventory.ID).ToList();
                    double price = (PriceSummary.Where(l => l.PurchasedDate >= report.DateStart && l.PurchasedDate <= report.DateEnd).Select(l => l.PurchasedCost)).Sum();

                    COGSSummaryTable.StartNewRow(); // Start a new row for each data set
                    COGSSummaryTable.AddCell(CreateTableCell(inventory.UPC.ToString(), textFont, textFontSize));
                    COGSSummaryTable.AddCell(CreateTableCell(inventory.Name, textFont, textFontSize));
                    COGSSummaryTable.AddCell(CreateTableCell(datestring, textFont, textFontSize));
                    COGSSummaryTable.AddCell(CreateTableCell(price.ToString("C"), textFont, textFontSize));
                }

                document.Add(COGSSummaryTable.SetMarginBottom(20));

                // Employee Logins
                var PricesTable = new Table(UnitValue.CreatePercentArray(new float[] { 1, 1, 1, 1, 1, 1 }));
                PricesTable.SetWidth(UnitValue.CreatePercentValue(100));

                // Added header row with no borders
                var pricesHeaderRow = new Cell(1, 5)
                    .SetBorder(Border.NO_BORDER)
                    .SetBackgroundColor(ColorConstants.LIGHT_GRAY)
                    .Add(new Paragraph("Items Summary")
                        .SetFont(subtitleFont)
                        .SetFontSize(subtitleFontSize)
                        .SetTextAlignment(TextAlignment.CENTER));

                PricesTable.AddHeaderCell(pricesHeaderRow);
                PricesTable.AddCell(CreateTableCell("UPC", subtitleFont, subtitleFontSize));
                PricesTable.AddCell(CreateTableCell("Name", subtitleFont, subtitleFontSize));
                PricesTable.AddCell(CreateTableCell("Cost", subtitleFont, subtitleFontSize));
                PricesTable.AddCell(CreateTableCell("COGS", subtitleFont, subtitleFontSize));
                PricesTable.AddCell(CreateTableCell("Gross Profit", subtitleFont, subtitleFontSize));
                PricesTable.AddCell(CreateTableCell("Profit Margin", subtitleFont, subtitleFontSize));

                /*
                foreach (var employee in report.HourlyReport.Employees.OrderBy(e => e.FullName))
                {
                    List<EmployeeLogin> logins = await _context.EmployeeLogins.Where(l => l.SignIn >= report.DateStart && l.SignIn <= report.DateEnd && l.EmployeeID == employee.ID).ToListAsync();
                    foreach (var login in logins)
                    {
                        employeeLoginTable.StartNewRow(); // Start a new row for each data set
                        employeeLoginTable.AddCell(CreateTableCell(employee.ID.ToString(), textFont, textFontSize));
                        employeeLoginTable.AddCell(CreateTableCell(employee.FullName, textFont, textFontSize));
                        employeeLoginTable.AddCell(CreateTableCell(login.SignIn.ToShortDateString(), textFont, textFontSize));
                        employeeLoginTable.AddCell(CreateTableCell((login.SignOut - login.SignIn).TotalHours.ToString(), textFont, textFontSize));
                        employeeLoginTable.AddCell(CreateTableCell(login.SignIn.ToShortTimeString(), textFont, textFontSize));
                        employeeLoginTable.AddCell(CreateTableCell(login.SignOut.ToShortTimeString(), textFont, textFontSize));
                    }
                }
                */
                
                document.Add(PricesTable.SetMarginBottom(20));



                // Footer (Report generated by {employee name} on {date})
                var footerTable = new Table(UnitValue.CreatePercentArray(new float[] { 1, 1, 1, 1, 1 }));
                footerTable.SetWidth(UnitValue.CreatePercentValue(100));
                footerTable.SetBorder(Border.NO_BORDER);
                footerTable.AddCell(CreateTableCell($"Report generated by Emma Ham on {report.DateCreated}", textFont, textFontSize));

                document.Add(footerTable);

                // Close the document
                document.Close();

                // Return the PDF as a byte array
                var pdfByteArray = memoryStream.ToArray();
                return File(pdfByteArray, "application/pdf", $"Report_COGS_{id}.pdf");
            }
        }

        public async Task<IActionResult> COGSReportDetails(int id)
        {
            var COGSReport = await _context.COGSReports.Include(c => c.Inventories)
                                                         .ThenInclude(p => p.Prices)
                                                         .Include(f => f.Invoices)
                                                         .ThenInclude(g => g.InvoiceLines)
                                                         .FirstOrDefaultAsync(s => s.ID == id);

            if (COGSReport == null)
            {
                return NotFound();
            }

            var viewModel = new COGSReportDetailsVM
            {
                COGSReport = COGSReport
            };
            
            return View(viewModel);
        }

        #endregion

    }
}

