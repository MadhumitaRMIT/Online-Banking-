using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Assignment2.Models;

namespace Assignment2.Models
{
    public interface IBankRepository
    {
        Task<int> Deposit(Account account);

        Task Withdraw(Account account);
    }
}
