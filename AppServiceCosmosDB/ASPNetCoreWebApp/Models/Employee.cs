using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AppServiceCosmosDB.Models
{
    public class Employee
    {
        [DisplayName("Id")]
        public string id { get; set; }

        [DisplayName("Employee Id")]
        public int? employeeId { get; set; }

        [Required]
        [DisplayName("Last Name")]
        public string lastName { get; set; }

        [Required]
        [DisplayName("First Name")]
        public string firstName { get; set; }

        [Required]
        [DisplayName("Address")]
        public string address { get; set; }
        [Required]
        [DisplayName("Zip Code")]
        public string zipCode { get; set; }
        [Required]
        [DisplayName("City")]
        public string city { get; set; }
        [Required]
        [DisplayName("Country")]
        public string country { get; set; }

    }
}
