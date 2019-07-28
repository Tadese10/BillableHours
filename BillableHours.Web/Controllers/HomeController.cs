using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using BillableHours.Web.Helper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Newtonsoft.Json;
using static BillableHours.Web.Helper.Help;

namespace BillableHours.Web.Controllers
{
    public class HomeController : Controller
    {
        public readonly HttpClient client = new HttpClient();
        public IActionResult SignUp()
        {
            return View(viewName: "Index");
        }

        [HttpPost]
        public async Task<ActionResult> SignUp(IFormCollection forms)
        {
            var message = string.Empty;
            bool isValid = true;
            var name = forms["name"].ToString();
            var email = forms["email"].ToString();
            var username = forms["username"].ToString();
            var pass = forms["pass"].ToString();
            var repeat_pass = forms["repeat-pass"].ToString();
            var ckb1 = forms["remember-me"].ToString();
            if (!ckb1.Equals("on"))
            {
                isValid = false;
                message += $"\tPlease accept terms of users,";
            }
            if (string.IsNullOrEmpty(name))
            {
                isValid = false;
                message += $"\t Please enter company name";
            }
            if (string.IsNullOrEmpty(email))
            {
                isValid = false;
                message += $"\t Please enter company email";
            }
            if (string.IsNullOrEmpty(username))
            {
                isValid = false;
                message += $"\t Please enter username";
            }
            if (string.IsNullOrEmpty(pass))
            {
                isValid = false;
                message += $"\t Please enter password";
            }
            if (string.IsNullOrEmpty(repeat_pass))
            {
                isValid = false;
                message += $"\t Please repeat your password.";
            }
            if (!pass.Equals(repeat_pass) && string.IsNullOrEmpty(repeat_pass) == false  && string.IsNullOrEmpty(pass) == false)
            {
                isValid = false;
                message += $"\t Password does not match.";
            }
            if (isValid)
            {
                var response = await Help.MakePostRequest<User>(HttpContext, $"v1/register", new
                {
                    CompanyName = name,
                    CompanyEmail = email,
                    username = username,
                    Password = pass
                });

                if (response.OK)
                {
                    ViewBag.Message = "";
                    return RedirectToAction("Login");
                }
                else
                {
                    ViewBag.Message = response.Message;
                    return View("Index");
                }

            }
            else
            {
                ViewBag.Message = message;
                return View("Index");
            }
            
        }

        public IActionResult Login()
        {
            var data = HttpContext.Session.Get("user")?.ToString();
            if (!string.IsNullOrEmpty(data))
            {
                return RedirectToAction("Index", "Dashboard");
            }
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Login(IFormCollection forms)
        {
            var message = string.Empty;
            bool isValid = true;
            var username = forms["username"].ToString();
            var pass = forms["pass"].ToString();
            if (string.IsNullOrEmpty(pass))
            {
                isValid = false;
                message += $"\t Please enter password";
            }
            if (string.IsNullOrEmpty(username))
            {
                isValid = false;
                message += $"\t Please enter username.";
            }
            if (isValid)
            {
                var response = await Help.MakePostRequest<User>(HttpContext, $"v1/authenticate", new
                {
                    username = username,
                    password = pass,
                });

                if (response.OK)
                {
                    ViewBag.Message = "";
                    HttpContext.Session.SetString("user",JsonConvert.SerializeObject(response.Data));
                    return RedirectToAction("Index", "Dashboard", response.Data);
                }
                else
                {
                    ViewBag.Message = response.Message;
                    return View("Login");
                }

            }
            else
            {
                ViewBag.Message = message;
                return View("Login");
            }
        }


    }
}