using BillableHours.Web.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using static BillableHours.Web.Helper.Help;

namespace BillableHours.Web.Controllers
{
    public class DashboardController : Controller
    {
        public async Task<IActionResult> Index(User user)
        {
            var userData = HttpContext.Session.GetString("user");
            if (userData == null)
            {
                return RedirectToAction("Login", "Home");
            }
            else
            {
                user = JsonConvert.DeserializeObject<User>(userData);
            }
            if (Request.Method == HttpMethods.Post)
            {
                if (!Request.Form.Files.Any())
                {
                    ViewBag.Message = "Please select a CSV file";
                    var mdata = await Helper.Help.MakePostRequest<List<Invoice>>(HttpContext, "v1/getInvoice", user.Username);
                    TempData["user"] = JsonConvert.SerializeObject(user).ToString();
                    return View("Dashboard", new DashbboardModel()
                    {
                        Invoices = mdata.Data
                    });
                }
                if (!Request.Form.Files[0].FileName.Contains(".csv"))
                {
                    ViewBag.Message = "Please select a CSV file";
                    var mdata = await Helper.Help.MakePostRequest<List<Invoice>>(HttpContext, "v1/getInvoice", user.Username);
                    TempData["user"] = JsonConvert.SerializeObject(user).ToString();
                    return View("Dashboard", new DashbboardModel()
                    {
                        Invoices = mdata.Data
                    });
                }
                var file = Request.Form.Files[0];
                using (var reader = new StreamReader(file.OpenReadStream()))
                {
                    var dat = await ProcessCSV(reader, user.Username ?? "");
                    if(dat.Count > 0)
                    {
                        var response = await Helper.Help.MakePostRequest<List<Invoice>>(HttpContext, "v1/saveInvoiceList", dat);
                        if (response.OK)
                        {
                            ViewBag.Message = "";
                        }
                        else
                        {
                            ViewBag.Message = response.Message ?? "An error occurred";
                        }
                        return View("Dashboard", new DashbboardModel()
                        {
                            Invoices = response.Data
                        });
                    }
                    else
                    {
                        ViewBag.Message = "Please select CSV that has data";
                        var res = await Helper.Help.MakePostRequest<List<Invoice>>(HttpContext, "v1/getInvoice", user);
                        return View("Dashboard", new DashbboardModel()
                        {
                            Invoices = res.Data
                        });
                    }
                }
            }

            ViewBag.Message = "";
            var data = await Helper.Help.MakePostRequest<List<Invoice>>(HttpContext, "v1/getInvoice", user);
            return View("Dashboard", new DashbboardModel()
            {
                Invoices = data.Data
            });
        }

        [HttpPost]
        public IActionResult Upload(List<IFormFile> files)
        {
            //if (model.BillFile!=null)
            //{
            //    var file = model.BillFile;

            //    using (var reader = new StreamReader(file.OpenReadStream()))
            //    {
            //        var read = ProcessCSV(reader);
            //    }
            //}
            //else
            //{

            //}
            ViewBag.Messsage = "test";
            return View("Dashboard");
        }

        private async static Task<List<Invoice>> ProcessCSV(StreamReader reader, string username)
        {
            string line = string.Empty;
            List<Invoice> invoiceList = new List<Invoice>();
            while ((line = await reader.ReadLineAsync()) != null)
            {
                try
                {
                    var data = line.Split(new char[] { ',' });
                    var start = data[3].Replace("\"", "");
                    var startTime = data[4].Replace("\"", "");
                    var endTime = data[5].Replace("\"", "");
                    var startDate = DateTime.Parse($"{int.Parse(start.Split('/')[1])}/{(int.Parse(start.Split('/')[0]))}/{GetYear(int.Parse(start.Split('/')[2]))}  {int.Parse(startTime.Split(':')[0])}:{int.Parse(startTime.Split(':')[1])}:0");
                    var endDate = DateTime.Parse($"{int.Parse(start.Split('/')[1])}/{(int.Parse(start.Split('/')[0]))}/{GetYear(int.Parse(start.Split('/')[2]))}  {int.Parse(endTime.Split(':')[0])}:{int.Parse(endTime.Split(':')[1])}:0");
                    invoiceList.Add(new Invoice
                    {
                        EmployeeId = data[0].Replace("\"", ""),
                        BillableRate = double.Parse(data[1].Replace("\"", "")),
                        Project = data[2],
                        StartDate = startDate,
                        EndDate = endDate,
                        username = username
                    });
                }
                catch (Exception ex)
                {

                }
            }

            //Tidy Streameader up
            reader.Dispose();

            return invoiceList;

        }

        private static string GetYear(int v)
        {
            if (v < 20)
            {
                return $"19{v}";
            }else
            {
                return $"20{v}";
            }
        }
    }
}