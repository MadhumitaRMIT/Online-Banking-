using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Assignment2.Data;
using Assignment2.Attributes;
using System;
using System.Collections.Generic;
using Assignment2.Models;
using Assignment2.Web;
using System.Net.Http;
using Microsoft.Extensions.Logging;
using X.PagedList;
using System.Linq;
using Assignment2.Utilities;

namespace Assignment2.Controllers
{
    [Route("admin/[controller]")]
    public class AdminbiilpayController : Controller
    {
        private readonly ILogger<AdminbiilpayController> _logger;
        private readonly asgn2Context _context;
        private BankApi _api = new BankApi();
        public AdminbiilpayController(asgn2Context context, ILogger<AdminbiilpayController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> BillPayList(int id, int? page = 1)
        {
            int CustomerID = 2100;
            _logger.LogInformation("First parameter. {0}", id);
            
            var customer = await _context.Customers.FindAsync(CustomerID);
            ViewBag.Customer = customer;
            
            const int pageSize = 10;
            
            var pagedList = await _context.BillPay.Where(x => x.AccountNumber == 4100).
                ToPagedListAsync(page, pageSize);
            
            //var pagedList = _context.BillPay.OrderBy(x => x.BillPayID).ToPagedListAsync(page, pageSize);

            return View(pagedList);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> ShowModifyBillPay(int id)
        {
            int CustomerID = 2100;
            var customer = await _context.Customers.FindAsync(CustomerID);
            ViewBag.Customer = customer;

            return View(await _context.BillPay.FindAsync(id));
        }


        [HttpPost]
        public async Task<IActionResult> UpdatebillPay(int billPayID, int AccountNumber, decimal amount, string schedule_date, int Period, bool IsBlocked)
        {
            BillPay billPay = await _context.BillPay.FindAsync(billPayID);
            
            if (amount <= 0)
                ModelState.AddModelError(nameof(amount), "Amount must be positive.");
            if (amount.HasMoreThanTwoDecimalPlaces())
                ModelState.AddModelError(nameof(amount), "Amount cannot have more than 2 decimal places.");
            if (!ModelState.IsValid)
            {
                ViewBag.Amount = amount;
                //return View(account);
                return RedirectToAction(nameof(ShowModifyBillPay));
            }

            try
            {
                var BankObj = new BankRepository(_context);
                var postId = await BankObj.UpdateBillPay(billPayID, AccountNumber, amount, schedule_date, Period, IsBlocked);
                if (postId > 0)
                {
                    //return Ok(postId);
                    //return RedirectToAction(nameof(Index));
                    return RedirectToAction("Index", "Admin");
                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception)
            {

                return BadRequest();
            }
        }








    }
}