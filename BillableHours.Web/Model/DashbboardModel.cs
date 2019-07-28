using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BillableHours.Web.Model
{
    public class DashbboardModel
    {
        public IFormFile BillFile { get; set; }
        public List<Invoice> Invoices { get; set; }
    }

    public class Invoice
    {
        public int Id { get; set; }
        public string EmployeeId { get; set; }
        public double BillableRate { get; set; }
        public string Project { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string username { get; set; }
        public double NumberOfHours { get; set; }
        public double Cost { get; set; }
    }
}
