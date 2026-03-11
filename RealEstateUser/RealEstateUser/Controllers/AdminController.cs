using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RealEstateUser.Models;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace RealEstateUser.Controllers
{
    public class AdminController : Controller
    {
        private readonly HttpClient _client;
        private readonly string apiUrl = "http://localhost:5127/api/Property"; 

        public AdminController(IHttpClientFactory httpClientFactory)
        {
            _client = httpClientFactory.CreateClient();
        }

        // Dashboard displaying pending properties
        public async Task<IActionResult> Index()
        {
            ViewBag.ActiveMenu = "Admin";

            string role = HttpContext.Session.GetString("Role");
            if (role != "Admin")
            {
                return RedirectToAction("Index", "Home");
            }

            string token = HttpContext.Session.GetString("AuthToken");
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "Auth");
            }

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            HttpResponseMessage response = await _client.GetAsync($"{apiUrl}/unapproved");

            List<PropertyModel> properties = new List<PropertyModel>();
            if (response.IsSuccessStatusCode)
            {
                var jsonResponse = await response.Content.ReadAsStringAsync();
                properties = JsonConvert.DeserializeObject<List<PropertyModel>>(jsonResponse);
            }
            else
            {
                ViewBag.ErrorMessage = "Failed to load pending properties for approval.";
            }

            return View(properties);
        }

        // Approve Property
        [HttpPost]
        public async Task<IActionResult> ApproveProperty(int id)
        {
            string token = HttpContext.Session.GetString("AuthToken");
            if (string.IsNullOrEmpty(token) || HttpContext.Session.GetString("Role") != "Admin")
            {
                return Unauthorized();
            }

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            HttpResponseMessage response = await _client.PutAsync($"{apiUrl}/approve/{id}", null);

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Property approved and listed on Home page.";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to approve property.";
            }

            return RedirectToAction("Index");
        }

        // Reject Property
        [HttpPost]
        public async Task<IActionResult> RejectProperty(int id)
        {
            string token = HttpContext.Session.GetString("AuthToken");
            if (string.IsNullOrEmpty(token) || HttpContext.Session.GetString("Role") != "Admin")
            {
                return Unauthorized();
            }

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            HttpResponseMessage response = await _client.PutAsync($"{apiUrl}/reject/{id}", null);

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Property rejected. The user will be notified in My Properties.";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to reject property.";
            }

            return RedirectToAction("Index");
        }
    }
}
