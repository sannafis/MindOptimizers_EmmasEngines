﻿using Microsoft.AspNetCore.Mvc;
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

namespace EmmasEngines.Controllers
{
    public class ReportsController : Controller
    {
        //add db context
        private readonly EmmasEnginesContext _context;

        public ReportsController(EmmasEnginesContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var viewModel = new ReportsVM
            {
                SalesReportVM = new SalesReportVM
                {
                    Employees = await _context.Employees.ToListAsync(),
                    SavedSalesReports = await _context.Reports.Where(r => r.Type == ReportType.Sales).ToListAsync(),
                    NewReport = new NewSalesReport()
                },
                Employees = await _context.Employees.ToListAsync()
                //Add in COGS and hourly here...
            };

            return View(viewModel);
        }


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
        public async Task<IActionResult> CreateSaleReport([FromForm] NewSalesReport newReport)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { success = false, message = "Invalid form data." });
            }

            try
            {
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
                _context.Reports.Add(new Report
                {
                    Description = "Sales Report",
                    DateStart = newReport.StartDate,
                    DateEnd = newReport.EndDate,
                    Criteria = newReport.AllEmployees ? "All Employees" : $"Employee: {newReport.EmployeeId}",
                    Type = 0,
                    DateCreated = DateTime.Now,
                    SalesReport = salesReport
                });

                
                
                
                await _context.SaveChangesAsync();

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                // Log the exception
                // _logger.LogError(ex, "Error creating a new sales report");

                return Json(new { success = false, message = "An error occurred while saving the report. Exception Message: " + ex.Message });
            }
        }




    }
}