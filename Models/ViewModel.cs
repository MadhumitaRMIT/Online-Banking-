using Microsoft.EntityFrameworkCore;
using Assignment2.Models;

namespace Assignment2.Models
{
    public class ViewModel
    {
        public Customer Customers { get; set; }
        public Account Accounts { get; set; }
        public BillPay BillPay { get; set; }

    }
}
