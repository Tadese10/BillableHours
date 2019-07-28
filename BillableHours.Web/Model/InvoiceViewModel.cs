using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static BillableHours.Web.Helper.Help;

namespace BillableHours.Web.Model
{
    public class InvoiceViewModel
    {
        public Invoice Invoice { get; set; }
        public User User { get; set; }
        public string InvoiceId { get; set; }
    }
}
