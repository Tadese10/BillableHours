using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BillableHours.Web.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using static BillableHours.Web.Helper.Help;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace BillableHours.Web.Controllers
{
    public class InvoiceController : Controller
    {
        // GET: /<controller>/
        public async Task<IActionResult> Index(int Id)
        {
            var userData = JsonConvert.DeserializeObject<User>(HttpContext.Session.GetString("user"));
            var mdata = await Helper.Help.MakePostRequest<Invoice>(HttpContext, "v1/getInvoiceById",  new Invoice() { Id = Id});
            mdata.Data.NumberOfHours = (mdata.Data.EndDate - mdata.Data.StartDate).TotalHours;
            mdata.Data.Cost = mdata.Data.NumberOfHours * mdata.Data.BillableRate;
            return View(new InvoiceViewModel()
            {
                Invoice = mdata.Data,
                InvoiceId = Guid.NewGuid().ToString().Substring(0,6),
                User= userData
            });
        }
    }
}
