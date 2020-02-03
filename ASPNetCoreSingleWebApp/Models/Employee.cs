using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AppServiceSingleCosmosDB.Models
{
    public class Employee
    {
        [DisplayName("Id")]
        public string id { get; set; }

        [Required]
        [DisplayName("unipersonid")]
        public int? unipersonid { get; set; }

        [Required]
        [DisplayName("employeeno")]
        public string employeeno { get; set; }

        [Required]
        [DisplayName("lastname")]
        public string lastname { get; set; }

        [Required]
        [DisplayName("Firstname")]
        public string Firstname { get; set; }

    }
}
