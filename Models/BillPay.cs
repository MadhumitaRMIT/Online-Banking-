using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

using System.Collections.Generic;

namespace Assignment2.Models
{
    public class BillPay
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int BillPayID { get; set; }

        public int AccountNumber { get; set; }

        public DateTime ModifyDate { get; set; }

        public int PayeeID { get; set; }
        public virtual List<Payee> Payee { get; set; }

        public DateTime ScheduleDate { get; set; }

        public int Period { get; set; }

        [Column(TypeName = "money")]
        public decimal Amount { get; set; }

        public bool IsBlocked { get; set; }


    }
}
