using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace BillableHours.Web.Helper
{
    public class Help
    {
       public static HttpResponseMessage MakeGetRequest(HttpContext request, string actionUrl)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(GetBaseUrl(request));
                    //HTTP GET
                    var responseTask = client.GetAsync(actionUrl);
                    responseTask.Wait();

                    return responseTask.Result;
                }
            }
            catch (Exception)
            {
                return null;
            }
            
        }

        public static async Task<Response<T>> MakePostRequest<T>(HttpContext request, string actionUrl, object data) where T : class
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(GetBaseUrl(request));

                    var responseData =  await client.PostAsJsonAsync(actionUrl, data);
                    return JsonConvert.DeserializeObject<Response<T>>(await responseData.Content.ReadAsStringAsync());
                }
            }
            catch (Exception ex)
            {
                return new Response<T>()
                {

                    OK = false
                };
            }

        }


        public static string GetBaseUrl(HttpContext request)
        {
            var host = request.Request.Host.ToUriComponent();

            var pathBase = request.Request.PathBase.ToUriComponent();

            //return $"{request.Request.Scheme}://{host}{pathBase}";
            return "https://localhost:44348/";
        }

        public class Response<T>
        {
            public string Message { get; set; }
            public bool OK { get; set; }
            public T Data { get; set; }
        }

        public class User
        {
            public int Id { get; set; }
            public string CompanyName { get; set; }
            public string CompanyEmail { get; set; }
            public string Username { get; set; }
            public byte[] PasswordHash { get; set; }
            public byte[] PasswordSalt { get; set; }
            public string Password { get; set; }
        }
    }
}
