using Assignment2.Data;
using Assignment2.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SimpleHashing;
using System;
using System.Threading.Tasks;


namespace NwbaExample.Controllers
{
    [Route("/Nwba/SecureLogin")]
    public class LoginController : Controller
    {
        private readonly asgn2Context _context;

        public LoginController(asgn2Context context) => _context = context;

        [HttpGet]
        public IActionResult Login()
        {
            Validation val = new Validation();

            try
            {
                if (HttpContext.Session.GetString("CustomerID") == null)
                {
                    return View();
                }
                else
                {
                    if (HttpContext.Session.GetString("CustomerID").Length == 0)
                    {
                        return View();
                    }
                    else
                    {

                        return RedirectToAction("Index", "Customer");
                    }
                }
            }
            catch (Exception)
            {
                ViewBag.ErrorTitle = "Login Page Get";
                ViewBag.ErrorMessage = "404: Login page not found...";
                return View("Error");
            }
        }




        [HttpPost]
        public async Task<IActionResult> Login(string loginID, string password)
        {
            var BankObj = new BankRepository(_context);
            try
            {

                var login = await _context.Logins.FindAsync(loginID);
                {

                    if (login.Counter == 3)
                    {
                        login.FailedAttempt = 1;
                        if (DateTime.Now.Subtract(login.AttemptTime).TotalMinutes > 1)
                        {
                            login.Counter = 0;
                            login.FailedAttempt = 0;
                        }
                        await BankObj.UpdateCred(login);
                    }

                    if ((!PBKDF2.Verify(login.PasswordHash, password)) && login.FailedAttempt == 0)
                    {
                        login.AttemptTime = DateTime.Now;
                        login.Counter = login.Counter + 1;
                        await BankObj.UpdateCred(login);
                        ModelState.AddModelError("LoginFailed", "Login failed, please try again.");
                        return View(new Login { LoginID = loginID });
                    }

                    // Login customer.
                    if (login.FailedAttempt == 1)
                    {
                        ModelState.AddModelError("LoginFailed", "Account locked, retry after 1 minute.");
                        return View(new Login { LoginID = loginID });
                    }
                    else
                    {
                        login.Counter = 0;
                        login.FailedAttempt = 0;
                        await BankObj.UpdateCred(login);
                        HttpContext.Session.SetInt32(nameof(Customer.CustomerID), login.CustomerID);
                        HttpContext.Session.SetString(nameof(Customer.Name), login.Customer.Name);
                        int LID = int.Parse(loginID);
                        HttpContext.Session.SetInt32("loginID", LID);

                        return RedirectToAction("Index", "Customer");
                    }
                }
            }
            catch (Exception)
            {
                ViewBag.ErrorTitle = "Login Page Post";
                ViewBag.ErrorMessage = "Error while submiting form for Login, Please try again...";
                return View("Error");
            }
        }




        [Route("LogoutNow")]
        [HttpGet]
        public IActionResult Logout()
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
                        HttpContext.Session.Clear();

                        return RedirectToAction("Index", "Home");
                    }
                    catch (Exception)
                    {
                        ViewBag.ErrorTitle = "Logout Page Get";
                        ViewBag.ErrorMessage = "404: Logout page not found...";
                        return View("Error");
                    }
                }
            }
        }


    }
}