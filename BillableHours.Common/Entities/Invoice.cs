using System;
using System.Collections.Generic;
using System.Text;

namespace BillableHours.Common.Entities
{
   public class Invoice
    {
        public int Id { get; set; }
        public string EmployeeId { get; set; }
        public decimal BillableRate { get; set; }
        public string Project { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string username { get; set; }
    }
}
