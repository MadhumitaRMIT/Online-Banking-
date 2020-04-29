using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Assignment2.Models
{
    public class Payee
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]

        public int PayeeID { get; set; }

        public string PayeeName { get; set; }

        public string Address { get; set; }

        public string City { get; set; }

        public string State { get; set; }

        public string PostCode { get; set; }

        public string Phone { get; set; }


    }
}
