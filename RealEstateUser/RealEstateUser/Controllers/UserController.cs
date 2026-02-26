using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RealEstateUser.Models;

namespace RealEstateUser.Controllers
{
    public class UserController : Controller
    {
        private readonly HttpClient _client;
        private readonly string apiUrl = "http://localhost:5127/api/User";

        public UserController(HttpClient client)
        {
            _client = client;
        }

        public IActionResult Index()
        {
            return View();
        }

        // GET: Display user profile using API
        public async Task<IActionResult> UserProfile()
        {
            string token = HttpContext.Session.GetString("AuthToken");
            string userIdString = HttpContext.Session.GetString("UserID");

            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "Auth");
            }

            if (!int.TryParse(userIdString, out int userId))
            {
                ViewBag.ErrorMessage = "User ID not found or invalid.";
                return View();
            }

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            HttpResponseMessage response = await _client.GetAsync($"{apiUrl}/{userId}");

            if (response.IsSuccessStatusCode)
            {
                var jsonResponse = await response.Content.ReadAsStringAsync();
                UserModel user = JsonConvert.DeserializeObject<UserModel>(jsonResponse);
                return View(user);
            }
            else
            {
                ViewBag.ErrorMessage = "Failed to retrieve user details.";
                return View();
            }
        }

        // GET: EditProfile - Display edit form with current user details
        public async Task<IActionResult> EditProfile()
        {
            string token = HttpContext.Session.GetString("AuthToken");
            string userIdString = HttpContext.Session.GetString("UserID");

            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "Auth");
            }

            if (!int.TryParse(userIdString, out int userId))
            {
                ViewBag.ErrorMessage = "User ID not found or invalid.";
                return View("UserProfile");
            }

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            HttpResponseMessage response = await _client.GetAsync($"{apiUrl}/{userId}");

            if (response.IsSuccessStatusCode)
            {
                var jsonResponse = await response.Content.ReadAsStringAsync();
                UserModel user = JsonConvert.DeserializeObject<UserModel>(jsonResponse);
               
                return View("UserProfile", user);
            }
            else
            {
                ViewBag.ErrorMessage = "Failed to retrieve user details.";
                return View("UserProfile");
            }
        }

        // POST: EditProfile - Process submitted form to update user details
        [HttpPost]
        public async Task<IActionResult> EditProfile(UserModel model)
        {
            string token = HttpContext.Session.GetString("AuthToken");
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "Auth");
            }

           
            if (string.IsNullOrEmpty(model.Password))
            {
                model.Password = HttpContext.Session.GetString("UserPassword");
            }

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

           
            var content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
            HttpResponseMessage response = await _client.PutAsync($"{apiUrl}/{model.UserID}", content);

            if (response.IsSuccessStatusCode)
            {
                
                return RedirectToAction("UserProfile");
            }
            else
            {
                ViewBag.ErrorMessage = "Failed to update user details.";
                return View("UserProfile", model);
            }
        }
    }
}
