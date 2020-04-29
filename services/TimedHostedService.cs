using System;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Assignment2.Data;
using Assignment2.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;

namespace Assignment2.services
{
    public class TimedHostedService : IHostedService, IDisposable
    {
        private readonly IServiceScopeFactory scopeFactory;
        private readonly asgn2Context _context;
        private int executionCount = 0;
        private readonly ILogger<TimedHostedService> _logger;
        private Timer _timer;

        public TimedHostedService(ILogger<TimedHostedService> logger, IServiceScopeFactory scopeFactory)
        {
            _logger = logger;
            this.scopeFactory = scopeFactory;
        }

        public Task StartAsync(CancellationToken stoppingToken)
        {
            //_logger.LogInformation("Timed Hosted Service running.");

            _timer = new Timer(DoWork, null, TimeSpan.Zero,
                TimeSpan.FromSeconds(5));
           
            return Task.CompletedTask;
        }

        private async void DoWork(object state)
        {
            var count = Interlocked.Increment(ref executionCount);
            /*
            _logger.LogInformation(
                "Timed Hosted Service is working. Count: {0}", DateTime.Now.ToString("MM/dd/yyyy 00:00:00"));
            */
            var toDate = DateTime.Now.ToString("dd/MM/yyyy");
            using (var scope = this.scopeFactory.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<asgn2Context>();
                var data = dbContext.BillPay
                    //.Where(a => a.IsBlocked == false)
                    .OrderBy(a => a.BillPayID);

                int[] toUpdate = new int[400];
                int counter = 0;
                try
                {
                    foreach (var order in data)
                    {
                        var schDate = order.ScheduleDate.ToString().Substring(0, 10);
                        if (schDate == toDate && !order.IsBlocked)
                        {
                            toUpdate[counter] = order.BillPayID;
                            counter++;
                        }
                    }
                } catch (Exception ex)
                {
                    _logger.LogInformation(" Something wrong in Service");
                }

                if(counter > 0)
                {
                    for (int runs = 0; runs < counter; runs++)
                    {
                        //_logger.LogInformation("to update {0}", toUpdate[runs]);
                        
                        var BillPayObj = await dbContext.BillPay.FindAsync(toUpdate[runs]);

                        var nextDate = DateTime.Today.AddDays(BillPayObj.Period).ToString("yyyy-MM-dd").Substring(0, 10);
                        DateTime oDate = Convert.ToDateTime(nextDate);

                        BillPayObj.ScheduleDate = oDate;

                        
                        Account account = await dbContext.Accounts.FindAsync(BillPayObj.AccountNumber);

                        try
                        {
                            if (BillPayObj.Amount > account.Balance) continue;
                        }
                        catch (NullReferenceException)
                        {

                        }
                        account.Balance -= BillPayObj.Amount;
                        account.Transactions.Add(
                            new Transaction
                            {
                                TransactionType = TransactionType.Withdraw,
                                Amount = BillPayObj.Amount,
                                TransactionTimeUtc = DateTime.UtcNow
                            });

                        await dbContext.SaveChangesAsync();
                    }
                }


            }

        }

        public Task StopAsync(CancellationToken stoppingToken)
        {
            //_logger.LogInformation("Timed Hosted Service is stopping.");

            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
