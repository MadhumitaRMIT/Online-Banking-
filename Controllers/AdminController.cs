using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Assignment2.Data;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using Assignment2.Models;
using Assignment2.Web;
using System.Net.Http;
using Microsoft.Extensions.Logging;
using System.Text;

namespace Assignment2.Controllers
{
    [Route("admin/[controller]")]
    public class AdminController : Controller
    {
        private readonly ILogger<AdminController> _logger;
        private readonly asgn2Context _context;
        private BankApi _api = new BankApi();
        public AdminController(asgn2Context context, ILogger<AdminController> logger) {
            _context = context;
            _logger = logger;
        }
        
        public IActionResult Index()
        {
            if (HttpContext.Session.GetString("AdminID") == null)
            {
                return RedirectToAction("Login", "Adminlogin");
            }
            else
            {
                if (HttpContext.Session.GetString("AdminID").Length == 0)
                {
                    return RedirectToAction("Login", "Adminlogin");
                }
                else
                {
                    return View();
                }
            }
        }
        
        [HttpGet]
        public async Task<IActionResult> Showuser()
        {
            if (HttpContext.Session.GetString("AdminID") == null)
            {
                return RedirectToAction("Login", "Adminlogin");
            }
            else
            {
                if (HttpContext.Session.GetString("AdminID").Length == 0)
                {
                    return RedirectToAction("Index", "Adminlogin");
                }
                else
                {
                    List<Customer> Customers = new List<Customer>();
                    HttpClient client =  _api.InitializeClient();

                    HttpResponseMessage res = await client.GetAsync("api/Bank");

                    if (res.IsSuccessStatusCode)
                    {
                        var results = res.Content.ReadAsStringAsync().Result;
                        Customers = JsonConvert.DeserializeObject<List<Customer>>(results);
                    }

                    return View(Customers);
                }
            }
        }

        [Route("[controller]/{id}")]
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            
            HttpClient client = _api.InitializeClient();
            HttpResponseMessage res = await client.GetAsync("api/Bank/"+id);
            //List<Customer> Customers = new List<Customer>();

            if (res.IsSuccessStatusCode)
            {
                var results = res.Content.ReadAsStringAsync().Result;
                var Customer = JsonConvert.DeserializeObject<Customer>(results);

                return View(Customer);
            }

            return NotFound();
            
        }

        [Route("[controller]/delete/{id}")]
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            _logger.LogInformation(" BEFORE DELETE . {0}", id);

            HttpClient client = _api.InitializeClient();

            HttpResponseMessage res = await client.DeleteAsync("api/Bank/" + id);

            if (res.IsSuccessStatusCode)
            {
                 var results = res.Content.ReadAsStringAsync().Result;
                 var Customers = JsonConvert.DeserializeObject<Customer>(results);
                 _logger.LogInformation(" AFTER Delete .");

                 RedirectToAction(nameof(Index));
            }
            else
            {
                _logger.LogInformation(" NOT SAVE . {0}", res);
            }

            return RedirectToAction("Showuser", "Admin");
            //return View('Showuser');
        }

        [Route("[controller]/view/{id}")]
        public async Task<IActionResult> Viewtransaction(int? id)
        {
            _logger.LogInformation(" Call FROM View Transaction. {0}", id);
            if (id == null)
            {
                return NotFound();
            }

            try
            {
                var customer = await _context.Customers.FindAsync(id);
                ViewBag.Customer = customer;

                HttpClient client = _api.InitializeClient();
                HttpResponseMessage res = await client.GetAsync("api/Accounts/" + id);
                List<Account> Accounts = new List<Account>();

                if (res.IsSuccessStatusCode)
                {
                    var results = res.Content.ReadAsStringAsync().Result;
                    Accounts = JsonConvert.DeserializeObject<List<Account>>(results);

                    return View(Accounts);
                }
            } catch (Exception ex)
            {
                _logger.LogInformation("Error in View Transaction.");
            }

            return View();
        }

        [Route("[controller]/transaction/{id}")]
        public async Task<IActionResult> Viewtransactionlist(int? id)
        {
            _logger.LogInformation(" Call FROM View Transaction1. {0}", id);
            if (id == null)
            {
                return NotFound();
            }

            try
            {
                HttpClient client = _api.InitializeClient();
                HttpResponseMessage res = await client.GetAsync("api/Transactions/" + id);
                List<Transaction> Transactions = new List<Transaction>();

                if (res.IsSuccessStatusCode)
                {
                    var results = res.Content.ReadAsStringAsync().Result;
                    Transactions = JsonConvert.DeserializeObject<List<Transaction>>(results);

                    return View(Transactions);
                }
            }
            catch (Exception ex)
            {
                _logger.LogInformation("Error in View Transaction.");
            }

            return View();
        }

        //int CustomerID, string Name, string Address, string City, string PostCode
        [HttpPost]
        public async Task<IActionResult> UpdateCustomer(Customer customer)
        {
            if (customer.Name == "") { 
                ModelState.AddModelError(nameof(customer.Name), "Name must not be null.");
                return RedirectToAction(nameof(Edit));
            }

            if (ModelState.IsValid)
            {
                _logger.LogInformation(" BEFORE POST . {0}", customer.Name);
                var content = new StringContent(JsonConvert.SerializeObject(customer), Encoding.UTF8, "application/json");
                _logger.LogInformation(" POST CONTENT . {0}", content);
                HttpClient client = _api.InitializeClient();
                //PostAsJsonAsync
                //PutAsync
                //client.DeleteAsync
                HttpResponseMessage res = await client.PostAsync("api/Bank/", content);

                if (res.IsSuccessStatusCode)
                {
                    var results = res.Content.ReadAsStringAsync().Result;
                    var Customers = JsonConvert.DeserializeObject<Customer>(results);
                    _logger.LogInformation(" AFTER SAVE . {0}", Customers.Name);

                    RedirectToAction(nameof(Index));
                }
                else
                {
                    _logger.LogInformation(" NOT SAVE . {0}", res);
                }
                    //return RedirectToAction("Index");
            }

            return RedirectToAction(nameof(Index));
        }

        /*
        [HttpPost]
        public async IActionResult Edit( Customer customer)
        {
            if (ModelState.IsValid)
            {
                var content = new StringContent(JsonConvert.SerializeObject(customer), Encoding.UTF8, "application/json");
                HttpClient client = _api.InitializeClient();
                HttpResponseMessage res = await client.PutAsync("api/Bank/", content);


                if (res.IsSuccessStatusCode)
                    return RedirectToAction("Index");
            }

            return View(customer);
        }
        */

    }
}