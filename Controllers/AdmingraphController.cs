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
using Newtonsoft.Json;
using System.Data;

namespace Assignment2.Controllers
{
    [Route("admin/[controller]")]
    public class AdmingraphController : Controller
    {
        private readonly ILogger<AdmingraphController> _logger;
        private readonly asgn2Context _context;
        private BankApi _api = new BankApi();

        public AdmingraphController(asgn2Context context, ILogger<AdmingraphController> logger)
        {
            _context = context;
            _logger = logger;
        }

        /*
        public IActionResult Index()
        {
            return View();
        }
        */

        [HttpGet]
        public async Task<IActionResult> Showgraph()
        {
            List<Customer> Customers = new List<Customer>();
            List<Transaction> Transaction = new List<Transaction>();
            List<Account> Account = new List<Account>();

            HttpClient client = _api.InitializeClient();

            HttpResponseMessage res = await client.GetAsync("api/Bank");

            if (res.IsSuccessStatusCode)
            {
                var results = res.Content.ReadAsStringAsync().Result;
                Customers = JsonConvert.DeserializeObject<List<Customer>>(results);
            }


            List<int> cust_city = new List<int>();
            var city = Customers.Select(x => x.City).Distinct().ToList();

            foreach (var item in city)
            {
                cust_city.Add(Customers.Count(x => x.City == item));
            }

            ViewBag.city = JsonConvert.SerializeObject(city.ToList());
            ViewBag.Rep = JsonConvert.SerializeObject(cust_city.ToList());

            //Deposit & Withdraw
            string[] Transactiontype = { "Withdraw", "Deposit" };
            HttpResponseMessage res1 = await client.GetAsync("api/Transactions");

            if (res1.IsSuccessStatusCode)
            {
                var results = res1.Content.ReadAsStringAsync().Result;
                _logger.LogInformation(" API CALL FROM TRANSACTION {0}", results);
                Transaction = JsonConvert.DeserializeObject<List<Transaction>>(results);
            }

            int no_of_deposit = 0;
            int no_of_withdraw = 0;
            foreach (var groupItem in Transaction)
            {
                var depositType = groupItem.TransactionType.ToString();
                // _logger.LogInformation(" TYPE: {0}", groupItem.TransactionType.ToString());

                if (depositType == "Withdraw") no_of_withdraw++;
                if (depositType == "Deposit") no_of_deposit++;

            }

            //List<int> no_of_deposit = new List<int>();
            //var no_of_withdraw = Transaction.Count(x => x.TransactionType.Equals("Withdraw"));

            //var no_of_deposit = Transaction.Count(x => x.TransactionType.Equals("Deposit"));
                
            _logger.LogInformation(" TOTAL DEPOSIT: {0}", no_of_deposit);
            _logger.LogInformation(" TOTAL WITHDRAW: {0}", no_of_withdraw);

            int[] terms = new int[2];
            terms[0] = no_of_withdraw;
            terms[1] = no_of_deposit;

            ViewBag.Transactiontype = JsonConvert.SerializeObject(Transactiontype);
            ViewBag.terms = JsonConvert.SerializeObject(terms);

            return View();
        }



        [HttpPost]
        public JsonResult NewChart()
        {
            List<object> iData = new List<object>();
            //Creating sample data  
            DataTable dt = new DataTable();
            dt.Columns.Add("Employee", System.Type.GetType("System.String"));
            dt.Columns.Add("Credit", System.Type.GetType("System.Int32"));

            DataRow dr = dt.NewRow();
            dr["Employee"] = "Sam";
            dr["Credit"] = 123;
            dt.Rows.Add(dr);

            dr = dt.NewRow();
            dr["Employee"] = "Alex";
            dr["Credit"] = 456;
            dt.Rows.Add(dr);

            dr = dt.NewRow();
            dr["Employee"] = "Michael";
            dr["Credit"] = 587;
            dt.Rows.Add(dr);
            //Looping and extracting each DataColumn to List<Object>  
            foreach (DataColumn dc in dt.Columns)
            {
                List<object> x = new List<object>();
                x = (from DataRow drr in dt.Rows select drr[dc.ColumnName]).ToList();
                iData.Add(x);
            }
            //Source data returned as JSON  
            return Json(iData);
        }




    }
}