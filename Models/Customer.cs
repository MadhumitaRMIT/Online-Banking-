using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace Assignment2.Models
{
    public class Customer
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int CustomerID { get; set; }

        [JsonProperty(PropertyName = "Name")]

        [Required, StringLength(50)]
        public string Name { get; set; }

        [StringLength(50)]
        public string Address { get; set; }

        [Required, StringLength(8)]
        public int Phone { get; set; }

        [StringLength(9)]
        public int TFN { get; set; }

        [JsonProperty(PropertyName = "City")]
        [StringLength(40)]
        public string City { get; set; }

        [StringLength(4)]
        public string PostCode { get; set; }

        [StringLength(3)]
        public string State { get; set; }

        public virtual List<Account> Accounts { get; set; }

    }
}
