using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AppServiceCosmosDB.Models
{
    public class Company
    {
        [DisplayName("Id")]
        public string id { get; set; }

        [DisplayName("Company Id")]
        public int? companyId { get; set; }

        [Required]
        [DisplayName("Name")]
        public string name { get; set; }


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
