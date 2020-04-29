using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Assignment2.Data;
using Assignment2.Models;
using SimpleHashing;


namespace Assignment2.Controllers
{
    [Route("admin/[controller]")]
    public class AdminloginController : Controller
    {
        private readonly asgn2Context _context;

        public AdminloginController(asgn2Context context) => _context = context;

        public IActionResult Login()
        {
            if (HttpContext.Session.GetString("AdminID") == null)
            {
                return View();
            }
            else
            {
                if (HttpContext.Session.GetString("AdminID").Length == 0)
                {
                    return View();
                }
                else
                {
                    return RedirectToAction("Index", "Admin");
                }
            }
        }

        [HttpPost]
        public async Task<IActionResult> Login(string loginID, string password)
        {
            //var login = await _context.Logins.FindAsync(loginID);

            if ((loginID == null || loginID != "Admin") && (password == null || password != "Admin"))
            {
                ModelState.AddModelError("LoginFailed", "Login failed, please try again.");
                return View(new Login { LoginID = loginID });
            }

            // Login Admin.
            HttpContext.Session.SetString("AdminID", loginID);

            return RedirectToAction("Index", "Admin");
        }

        [Route("LogoutNow")]
        public IActionResult Logout()
        {
            if (HttpContext.Session.GetString("AdminID") == null)
            {
                return RedirectToAction("Index", "Admin");
            }
            else
            {
                if (HttpContext.Session.GetString("AdminID").Length == 0)
                {
                    return RedirectToAction("Index", "Admin");
                }
                else
                {
                    HttpContext.Session.Clear();

                    return RedirectToAction("Index", "Admin");
                }

            }

        }
    }
}