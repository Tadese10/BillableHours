using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BillableHours.Api.Services;
using BillableHours.Common.Entities;
using BillableHours.Common.Helper;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace BillableHours.Api.Controllers
{
    [Route("v1/")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        public readonly IUserService userService;
        public UsersController(IUserService _userService)
        {
            userService = _userService;
        }

        [HttpPost]
        [Route("authenticate")]
        public ActionResult<Response<User>> AuthenticateUser(User userModel)
        {
           return  userService.Authenticate(userModel.Username, userModel.Password);
        }

        [HttpPost]
        [Route("register")]
        public ActionResult<Response<User>> RegisterUser(User user)
        {
            return userService.Create(user,user.Password);
        }

        [HttpPost]
        [Route("getInvoice")]
        public ActionResult<Response<List<Invoice>>> GetInvoice(User username)
        {
            var data =  userService.GetInvoiceList(username.Username);
            return data;
        }

        [HttpPost]
        [Route("getInvoiceById")]
        public ActionResult<Response<Invoice>> GetInvoiceById(Invoice invoice)
        {
            var data = userService.GetInvoiceById(invoice.Id);
            return data;
        }

        [HttpPost]
        [Route("saveInvoiceList")]
        public ActionResult<Response<List<Invoice>>> AddInvoice(object invoice)
        {
            JArray jsonResponse = JArray.Parse(invoice.ToString());
            var mInvoice = new List<Invoice>();
            foreach (var item in jsonResponse)
            {
                    JObject rItemValueJson = (JObject)item;
                    Invoice rowsResult = item.ToObject<Invoice>();
                    mInvoice.Add(rowsResult);
            }
            
            return userService.AddInvoiceList(mInvoice);
        }

      
    }
}
