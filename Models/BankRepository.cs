using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Assignment2.Data;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;

 // using Microsoft.Ajax.Utilities;

namespace Assignment2.Models
{
    public class BankRepository 
    {

        private readonly asgn2Context _context;
        private readonly ILogger<BankRepository> _logger;

        public BankRepository(asgn2Context context)
        {
            _context = context;
            
        }

        public async Task<int> Deposit(Account account, decimal amount)
        {
            if (_context != null)
            {
                account.Balance += amount;
                account.Transactions.Add(
                    new Transaction
                    {
                        TransactionType = TransactionType.Deposit,
                        Amount = amount,
                        TransactionTimeUtc = DateTime.UtcNow
                    });
                await _context.SaveChangesAsync();
                return 1;
            }

            return 0;
        }

        public async Task<int> Withdraw(Account account, decimal amount)
        {
            if (_context != null)
            {
                ///Check  amount with Balance
                account.Balance -= amount;
                account.Transactions.Add(
                    new Transaction
                    {
                        TransactionType = TransactionType.Withdraw,
                        Amount = amount,
                        TransactionTimeUtc = DateTime.UtcNow
                    });

                await _context.SaveChangesAsync();
                return 1;
            }

            return 0;
        }

        public async Task<int> BillPay(int id, decimal amount, string schedule_date, int Period)
        {
            if (_context != null)
            {
                string iDate = schedule_date;
                DateTime oDate = Convert.ToDateTime(iDate);

                var BillPay = new BillPay
                {
                    AccountNumber = id,
                    ModifyDate = DateTime.UtcNow,
                    Amount = amount,
                    ScheduleDate = oDate,
                    PayeeID = 1,
                    Period = Period,
                };

                _context.Add(BillPay);
                _context.SaveChanges();
                //await _context.SaveChangesAsync();
                return 1;
            }

            return 0;
        }

        
        public async Task<int> UpdateBillPay(int id, int AccountNumber, decimal amount, string schedule_date, int Period, bool IsBlocked)
        {
            if (_context != null)
            {
                string iDate = schedule_date;
                DateTime oDate = Convert.ToDateTime(iDate);

                var BillPayObj = await _context.BillPay.FindAsync(id);


                BillPayObj.AccountNumber = AccountNumber;
                BillPayObj.ModifyDate = DateTime.UtcNow;
                BillPayObj.Amount = amount;
                BillPayObj.ScheduleDate = oDate;
                BillPayObj.PayeeID = 1;
                BillPayObj.Period = Period;
                BillPayObj.IsBlocked = IsBlocked;



                //_context.SaveChanges();
                await _context.SaveChangesAsync();
                return 1;
            }

            return 0;
        }
        public async Task<int> DeleteBillPay(int id)
        {
            if (_context != null)
            {
                var BillPay = new BillPay { BillPayID = id };
                _context.Remove(BillPay);
                _context.SaveChanges();
                return 1;
            }
            
            return 0;
        }

        //public async Task<int> GetScheduleTask() public async Task<List<TEntity>> GetAll()
        public async Task<int> ProcessScheduleTask()
        {
            //return 100;
            var data = _context.BillPay.OrderBy(a => a.BillPayID);
            //var data = _context.BillPay.Where(p => p.BillPayID > 1);
            //var data = await _context.QueryAsync<BillPay>("SELECT * FROM BillPay");
            //return data.ToArray();

            //BillPay context = new BillPay();
            //.Set<TEntity>().ToListAsync();
            //return context;

            //return _context.BillPay.Where(p => p.BillPayID > 1);



            foreach (var order in data)
            {
                Console.WriteLine(order.BillPayID);
                _logger.LogInformation("FROM REPOSITORY {0}", order.BillPayID);
            }
            //_logger.LogInformation("FROM REPOSITORY {0}", data);
            return 100;


        }

        // Update Customer Details
        public async Task<int> UpdateCustDet(Customer cust) {

            if (cust.Name.Length!=0) {
                _context.Update(cust);
                _context.SaveChanges();

                await _context.SaveChangesAsync();
                return 1;
            }
            return 0;
        }


        // Update Credentials
        public async Task<int> UpdateCred(Login log)
        {

            if (log.LoginID.ToString().Length != 0)
            {
                _context.Update(log);
                _context.SaveChanges();

                await _context.SaveChangesAsync();
                return 1;
            }
            return 0;
        }
    }
}
