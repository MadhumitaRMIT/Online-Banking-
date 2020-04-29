using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Assignment2.Models
{
    public class Login
    {

        [Required, StringLength(8)]
        [Display(Name = "Login ID")]
        
        public string LoginID { get; set; }

        public int CustomerID { get; set; }
        
        public virtual Customer Customer { get; set; }

        [Required, StringLength(64)]
        public string PasswordHash { get; set; }

        public int Counter { get; set; }

        public int FailedAttempt { get; set; }

        public DateTime AttemptTime { get; set; }

    }
}
