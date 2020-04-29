using Assignment2.Data;
using Assignment2.Models;
using Assignment2.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using Assignment2.Attributes;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;
using X.PagedList;

namespace Assignment2.Controllers
{
    [AuthorizeCustomer]
    public class CustomerController : Controller
    {
        private readonly asgn2Context _context;
        private readonly ILogger<CustomerController> _logger;
        private Timer _timer;

        // ReSharper disable once PossibleInvalidOperationException
        private int CustomerID => HttpContext.Session.GetInt32(nameof(Customer.CustomerID)).Value;

        public CustomerController(asgn2Context context, ILogger<CustomerController> logger)
        {
            _context = context;
            _logger = logger;
        }
        

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            if (HttpContext.Session.GetString("CustomerID") == null)
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                if (HttpContext.Session.GetString("CustomerID").Length == 0)
                {
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    try
                    {
                        if (HttpContext.Session.GetString("CustomerID").Length == 0)
                        {
                            return RedirectToAction("Index", "Home");
                        }
                        else
                        {
                            var customer = await _context.Customers.FindAsync(CustomerID);

                            return View(customer);
                        }
                    }
                    catch (Exception)
                    {
                        ViewBag.ErrorTitle = "Home page not reachable.";
                        ViewBag.ErrorMessage = "404: Site Unreachable, please try again...";
                        return View("Error");
                    }
                }
            }
        }



        [HttpGet]
        public async Task<IActionResult> Deposit(int id)
        {
            if (HttpContext.Session.GetString("CustomerID") == null)
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                try
                {
                    if (HttpContext.Session.GetString("CustomerID").Length == 0)
                    {
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        return View(await _context.Accounts.FindAsync(id));
                    }
                }
                catch (Exception)
                {
                    ViewBag.ErrorTitle = "Deposit Page Get.";
                    ViewBag.ErrorMessage = "404: Deposit page not found...";
                    return View("Error");
                }
            }
        }



        [HttpPost]
        public async Task<IActionResult> Deposit(int id, decimal amount)
        {
            Account account = await _context.Accounts.FindAsync(id);

            if (amount <= 0)
                ModelState.AddModelError(nameof(amount), "Amount must be positive.");
            if (amount.HasMoreThanTwoDecimalPlaces())
                ModelState.AddModelError(nameof(amount), "Amount cannot have more than 2 decimal places.");
            if (!ModelState.IsValid)
            {
                ViewBag.Amount = amount;
                return View(account);
            }

            try
            {
                var BankObj = new BankRepository(_context);
                var postId = await BankObj.Deposit(account, amount);
                if (postId > 0)
                {
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception)
            {
                ViewBag.ErrorTitle = "Deposit Post.";
                ViewBag.ErrorMessage = "Error in Deposit form. Plaese try again...";

                return View("Error");
            }
        }



        [HttpGet]
        public async Task<IActionResult> Withdraw(int id)
        {

            if (HttpContext.Session.GetString("CustomerID") == null)
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                try
                {
                    if (HttpContext.Session.GetString("CustomerID").Length == 0)
                    {
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        return View(await _context.Accounts.FindAsync(id));
                    }
                }
                catch (Exception)
                {
                    ViewBag.ErrorTitle = "Withdraw Page Get";
                    ViewBag.ErrorMessage = "404: Withdraw page not found...";
                    return View("Error");
                }
            }
        }



        [HttpPost]
        public async Task<IActionResult> Withdraw(int id, decimal amount)
        {
            Account account = await _context.Accounts.FindAsync(id);

            if (amount <= 0)
                ModelState.AddModelError(nameof(amount), "Amount must be positive.");
            if (amount.HasMoreThanTwoDecimalPlaces())
                ModelState.AddModelError(nameof(amount), "Amount cannot have more than 2 decimal places.");
            if (!ModelState.IsValid)
            {
                ViewBag.Amount = amount;
                return View(account);
            }

            try
            {
                var BankObj = new BankRepository(_context);
                var postId = await BankObj.Withdraw(account, amount);
                if (postId > 0)
                {
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception)
            {
                ViewBag.ErrorTitle = "Withdraw Page Get";
                ViewBag.ErrorMessage = "Error in Deposit form. Plaese try again...";
                return View("Error");
            }
        }



        [HttpGet]
        public async Task<IActionResult> Transfer(int id)
        {
            if (HttpContext.Session.GetString("CustomerID") == null)
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                try
                {
                    if (HttpContext.Session.GetString("CustomerID").Length == 0)
                    {
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        return View(await _context.Accounts.FindAsync(id));
                    }
                }
                catch (Exception)
                {
                    ViewBag.ErrorTitle = "Transfer Page Get";
                    ViewBag.ErrorMessage = "404: Withdraw page not found...";
                    return View("Error");
                }
            }
        }


        [HttpPost]
        public async Task<IActionResult> Transfer(int id, decimal amount)
        {
            Account account = await _context.Accounts.FindAsync(id);

            if (amount <= 0)
                ModelState.AddModelError(nameof(amount), "Amount must be positive.");
            if (amount.HasMoreThanTwoDecimalPlaces())
                ModelState.AddModelError(nameof(amount), "Amount cannot have more than 2 decimal places.");
            try
            {
                if (!ModelState.IsValid)
                {
                    ViewBag.Amount = amount;
                    return View(account);
                }
                account.Balance -= amount;
                account.Transactions.Add(
                    new Transaction
                    {
                        TransactionType = TransactionType.Withdraw,
                        Amount = amount,
                        TransactionTimeUtc = DateTime.UtcNow
                    });

                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            catch (Exception)
            {
                ViewBag.ErrorTitle = "Transfer Page Post";
                ViewBag.ErrorMessage = "404: Transfer page not found...";
                return View("Error");
            }
        }


        [HttpGet]
        public async Task<IActionResult> Paybill()
        {
            try
            {
                var customer = await _context.Customers.FindAsync(CustomerID);
                return View(customer);
            }
            catch (Exception)
            {
                ViewBag.ErrorTitle = "Paybill Page Get";
                ViewBag.ErrorMessage = "404: Paybill page not found...";
                return View("Error");
            }
        }


        [HttpPost]
        public async Task<IActionResult> PaybillSave(int from_account, decimal amount, string schedule_date, int Period)
        {
            _logger.LogInformation("First parameter. {0}", from_account);
            _logger.LogInformation("Second parameter. {0}", amount);
            _logger.LogInformation("Third parameter. {0}", Period);

            
            Account account = await _context.Accounts.FindAsync(from_account);

            if (amount <= 0)
                ModelState.AddModelError(nameof(amount), "Amount must be positive.");
            if (amount.HasMoreThanTwoDecimalPlaces())
                ModelState.AddModelError(nameof(amount), "Amount cannot have more than 2 decimal places.");
            if (!ModelState.IsValid)
            {
                ViewBag.Amount = amount;
                return RedirectToAction(nameof(Paybill));
            }

            try
            {
                var BankObj = new BankRepository(_context);
                var postId = await BankObj.BillPay(from_account, amount, schedule_date, Period);
                if (postId > 0)
                {
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    ViewBag.ErrorTitle = "PayBill Save Page Post";
                    ViewBag.ErrorMessage = "404: Error in PayBill Form, plaese try again...";
                    return View("Error");
                }
            }
            catch (Exception)
            {
                ViewBag.ErrorTitle = "PayBill Save Page Post";
                ViewBag.ErrorMessage = "404: Error in PayBill Form, plaese try again...";
                return View("Error");
            }
        }


        [HttpGet]
        public async Task<IActionResult> BillPayList(int id, int? page = 1)
        {
            try
            {
                _logger.LogInformation("First parameter. {0}", id);
                var customer = await _context.Customers.FindAsync(CustomerID);
                ViewBag.Customer = customer;
                const int pageSize = 10;

                var pagedList = await _context.BillPay.Where(x => x.AccountNumber == id).
                    ToPagedListAsync(page, pageSize);

                return View(pagedList);
            }
            catch (Exception)
            {
                ViewBag.ErrorTitle = "BillPayList Page Get";
                ViewBag.ErrorMessage = "404: BillPayList page not found...";
                return View("Error");
            }
        }


        [HttpGet]
        public async Task<IActionResult> ShowModifyBillPay(int id)
        {
            try
            {
                var customer = await _context.Customers.FindAsync(CustomerID);
                ViewBag.Customer = customer;
                return View(await _context.BillPay.FindAsync(id));
            }
            catch (Exception)
            {
                ViewBag.ErrorTitle = "ShowModifyBillPay Page Get";
                ViewBag.ErrorMessage = "404: ModifyBillPay page not found...";
                return View("Error");
            }
        }


        [HttpPost]
        public async Task<IActionResult> UpdatebillPay(int billPayID ,int from_account, decimal amount, string schedule_date, int Period)
        {
            BillPay billPay = await _context.BillPay.FindAsync(billPayID);

            if (amount <= 0)
                ModelState.AddModelError(nameof(amount), "Amount must be positive.");
            if (amount.HasMoreThanTwoDecimalPlaces())
                ModelState.AddModelError(nameof(amount), "Amount cannot have more than 2 decimal places.");
            if (!ModelState.IsValid)
            {
                ViewBag.Amount = amount;
                return RedirectToAction(nameof(ShowModifyBillPay));
            }

            try
            {
                var BankObj = new BankRepository(_context);
                var postId = await BankObj.UpdateBillPay(billPayID, from_account, amount, schedule_date, Period, false);
                if (postId > 0)
                {
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    ViewBag.ErrorTitle = "UpdateBillPay Page Post";
                    ViewBag.ErrorMessage = "404: Update Bill Pay page not found...";
                    return View("Error");
                }
            }
            catch (Exception)
            {
                ViewBag.ErrorTitle = "UpdateBillPay Page Post";
                ViewBag.ErrorMessage = "404: Update Bill Pay page not found...";
                return View("Error");
            }
        }


        [HttpGet]
        public async Task<IActionResult> Statement(int id, int? page = 1 )
        {
            try
            {
                var customer = await _context.Customers.FindAsync(CustomerID);
                ViewBag.Customer = customer;
                const int pageSize = 10;

                var pagedList = await _context.Transactions.Where(x => x.AccountNumber == id).
                    ToPagedListAsync(page, pageSize);

                return View(pagedList);
            }
            catch (Exception)
            {
                ViewBag.ErrorTitle = "Statement Page Get";
                ViewBag.ErrorMessage = "404: Statement page not found...";
                return View("Error");
            }
        }



        [HttpGet]
        public async Task<IActionResult> DeleteBillPay(int id)
        {
            var customer = await _context.Customers.FindAsync(CustomerID);
            try
            {
                var BankObj = new BankRepository(_context);
                var postId = await BankObj.DeleteBillPay(id);
                if (postId > 0)
                {
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    ViewBag.ErrorTitle = "DeleteBillPay Page Get";
                    ViewBag.ErrorMessage = "404: Delete Bill Pay page not found...";
                    return View("Error");
                }
            }
            catch (Exception)
            {
                ViewBag.ErrorTitle = "DeleteBillPay Page Get";
                ViewBag.ErrorMessage = "404: Delete Bill Pay page not found...";
                return View("Error");
            }
        }



        // Snippet Edit Details...
        [HttpGet]
        public async Task<IActionResult> EditDetails(int id)
        {

            if (HttpContext.Session.GetString("CustomerID") == null)
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                try
                {
                    if (HttpContext.Session.GetString("CustomerID").Length == 0)
                    {
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        return View(await _context.Accounts.FindAsync(id));
                    }
                }
                catch (Exception)
                {
                    ViewBag.ErrorTitle = "EditDetails Page Get";
                    ViewBag.ErrorMessage = "404: Edit Details page not found...";
                    return View("Error");
                }
            }
        }



        [HttpPost]
        public async Task<IActionResult> EditDetails(String Name, String Address, String City, String State, int PostCode, int Phone, int TFN)
        {

            Customer cust = new Customer();
            Validation val = new Validation();

            ViewBag.Name = Name;
            ViewBag.Address = Address;
            ViewBag.City = City;
            ViewBag.Phone = Phone;
            ViewBag.PostCode = PostCode;
            ViewBag.State = State;
            ViewBag.TFN = TFN;

            // Name
            if (Name.Length < 1 || Name.Length > 50)
            {
                ModelState.AddModelError(nameof(Name), "Name must have a value between 1 to 50 characters.");
                return View();
            }
            else
            {
                if (val.NameVal(Name))
                {
                    Name = ViewBag.Name;
                }
                else
                {
                    ModelState.AddModelError("Name", "Special charecters and numbers are not allowed in a name.");
                    return View();
                }
            }

            // Address
            if (Address.Length == 0 || Address.Length > 50)
            {

                ModelState.AddModelError(nameof(Address), "Address must have a value between 1 to 50 characters.");
                return View();
            }
            else
            {
                Address = ViewBag.Address;
            }


            // City
            if (City.Length == 0 || City.Length > 40)
            {

                ModelState.AddModelError(nameof(City), "City must have a value between 1 to 40 characters");
                return View();
            }
            else
            {
                if (val.NameVal(City))
                {
                    City = ViewBag.City;
                }
                else
                {
                    ModelState.AddModelError(nameof(City), "Special charecters are not allowed in a name of the city.");
                }
            }

            String testNum = Phone.ToString();

            if (testNum.Length >= 13 || testNum.Length == 0)
            {

                ModelState.AddModelError(nameof(Phone), "Phone must have a value between 9 - 12 digits.");
                return View();
            }
            else
            {
                if (val.NumberVal(testNum))
                {
                    Phone = ViewBag.Phone;
                }
                else
                {
                    ModelState.AddModelError(nameof(Phone), "Please enter a valid phone number.");
                }
            }


            // PostCode

            String testPst = PostCode.ToString();

            if (testPst.Length >= 5 || testPst.Length == 0)
            {

                ModelState.AddModelError(nameof(PostCode), "Post Code must have a value of 4 digits only.");
                return View();
            }
            else
            {
                if (val.NumberVal(testPst))
                {
                    PostCode = ViewBag.PostCode;
                }
                else
                {
                    ModelState.AddModelError(nameof(PostCode), "Please enter a valid Post Code.");
                }
            }

            // State
            if (State.Length != 3)
            {
                ModelState.AddModelError(nameof(State), "State must have a value.");
                return View();
            }
            else
            {
                if (val.NameVal(State))
                {
                    State = ViewBag.State;
                }
                else
                {
                    ModelState.AddModelError("State", "Special charecters and numbers are not allowed in a State.");
                    return View();
                }
            }

            // TFN
            if (TFN.ToString().Length > 11 || TFN.ToString().Length < 9)
            {
                ModelState.AddModelError(nameof(TFN), "TFN must have 9 to 11 digits.");
                return View();
            }
            else
            {
                if (val.NumberVal(TFN.ToString()))
                {
                    TFN = ViewBag.TFN;
                }
                else
                {
                    ModelState.AddModelError("TFN", "Special charecters are not allowed in a TFN.");
                    return View();
                }
            }

            // TFN
            if (TFN.ToString().Length <= 8 || TFN.ToString().Length >= 10)
            {
                ModelState.AddModelError(nameof(TFN), "TFN must have 9 digits.");
                return View();
            }
            else
            {
                if (val.NumberVal(TFN.ToString()))
                {
                    TFN = ViewBag.TFN;
                }
                else
                {
                    ModelState.AddModelError("TFN", "Special charecters are not allowed in a TFN.");
                    return View();
                }
            }


            cust.Name = Name;
            cust.Address = Address;
            cust.City = City;
            cust.Phone = Phone;
            cust.State = State;
            cust.TFN = TFN;
            cust.PostCode = PostCode.ToString();


            cust.CustomerID = Convert.ToInt32(HttpContext.Session.GetInt32("CustomerID"));

            var BankObj = new BankRepository(_context);
            var update = await BankObj.UpdateCustDet(cust);

            try
            {
                if (update == 1)
                {

                    await _context.SaveChangesAsync();

                    return RedirectToAction(nameof(Index));
                }
                return RedirectToAction(nameof(Index));
            }
            catch (Exception)
            {
                ViewBag.ErrorTitle = "EditDetails Page Post";
                ViewBag.ErrorMessage = "Error while submiting form for Edit Details, Please try again...";
                return View("Error");
            }
        }



        // ViewDetails
        [HttpGet]
        public async Task<IActionResult> ViewDetails(int id)
        {

            if (HttpContext.Session.GetString("CustomerID") == null)
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                try
                {
                    if (HttpContext.Session.GetString("CustomerID").Length == 0)
                    {
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        var customer = await _context.Customers.FindAsync(CustomerID);

                        return View(customer);
                    }
                }
                catch (Exception)
                {
                    ViewBag.ErrorTitle = "ViewDetails Page Get";
                    ViewBag.ErrorMessage = "404: View Details page not found...";
                    return View("Error");
                }
            }
        }



        // Credentials
        [HttpGet]
        public async Task<IActionResult> Credentials(int id)
        {
            if (HttpContext.Session.GetString("CustomerID") == null)
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                try
                {
                    if (HttpContext.Session.GetString("CustomerID").Length == 0)
                    {
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {

                        return View();
                    }
                }
                catch (Exception)
                {
                    ViewBag.ErrorTitle = "Credentials Page Get";
                    ViewBag.ErrorMessage = "404: Credentials page not found...";
                    return View("Error");
                }
            }
        }



        [HttpPost]
        public async Task<IActionResult> Credentials(String Password, String ConfirmPAssword)
        {
            int LoginID = Convert.ToInt32(HttpContext.Session.GetInt32("loginID"));

            Validation val = new Validation();

            ViewBag.Password = Password;
            ViewBag.ConfirmPAssword = ConfirmPAssword;

            // Password
            if (Password.Length < 1 || Password.Length > 50)
            {
                if (!Password.Equals(ConfirmPAssword))
                    ModelState.AddModelError(nameof(Password), "Password must have a value between 1 to 50 characters and Passwords should match.");
                return View();
            }
            else
            {
                try
                {
                    if (Password.Length != 0 && Password.Equals(ConfirmPAssword))
                    {

                        if ((val.CreateHash(Password)).Length != 0)
                        {

                            Login log = new Login();

                            log.LoginID = LoginID.ToString();
                            log.PasswordHash = val.CreateHash(Password);

                            int id = Convert.ToInt32(HttpContext.Session.GetInt32("CustomerID"));

                            log.CustomerID = id;

                            var BankObj = new BankRepository(_context);
                            var update = await BankObj.UpdateCred(log);

                            if (update == 1)
                            {

                                await _context.SaveChangesAsync();

                                return RedirectToAction(nameof(Index));
                            }
                            return RedirectToAction(nameof(Index));

                        }
                    }
                }
                catch (Exception)
                {
                    ViewBag.ErrorTitle = "Credentials Page Post";
                    ViewBag.ErrorMessage = "Error in Credentials form. Please try again...";
                    return View("Error");
                }            
            }
            return RedirectToAction(nameof(Index));
        }
    }
}