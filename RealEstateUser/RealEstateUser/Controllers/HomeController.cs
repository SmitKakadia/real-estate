using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RealEstateUser.Models;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Linq;

namespace RealEstateUser.Controllers
{
    public class HomeController : Controller
    {
        private readonly HttpClient _client;
        private readonly string apiUrl = "http://localhost:5127/api/Property"; 

        public HomeController(IHttpClientFactory httpClientFactory)
        {
            _client = httpClientFactory.CreateClient();
        }

        // Fetch all properties securely
        public async Task<IActionResult> Index()
        {
            ViewBag.ActiveMenu = "Home";
            List<PropertyModel> properties = new List<PropertyModel>();

            try
            {
                string token = HttpContext.Session.GetString("AuthToken");
                if (string.IsNullOrEmpty(token))
                {
                    // Redirect to login if token is missing
                    return RedirectToAction("Login", "Auth");
                }

                _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                HttpResponseMessage response = await _client.GetAsync(apiUrl);

                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    properties = JsonConvert.DeserializeObject<List<PropertyModel>>(jsonResponse);
                }
                else
                {
                    ViewBag.ErrorMessage = "Failed to retrieve properties.";
                }
            }
            catch (System.Exception ex)
            {
                System.Console.WriteLine($"Error fetching data: {ex.Message}");
                ViewBag.ErrorMessage = "Error fetching data.";
            }

            return View(properties);
        }

        [HttpGet("Home/PropertyDetail/{id}")]
        public async Task<IActionResult> PropertyDetail(int id)
        {
            try
            {
                string token = HttpContext.Session.GetString("AuthToken");
                if (string.IsNullOrEmpty(token))
                {
                    return RedirectToAction("Login", "Auth");
                }

                _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                HttpResponseMessage response = await _client.GetAsync($"{apiUrl}/{id}");

                if (!response.IsSuccessStatusCode)
                {
                    return NotFound();
                }

                string jsonResponse = await response.Content.ReadAsStringAsync();
                var property = JsonConvert.DeserializeObject<PropertyModel>(jsonResponse);

                if (property == null)
                {
                    return NotFound();
                }

                return View(property);
            }
            catch (System.Exception ex)
            {
                System.Console.WriteLine($"Error fetching property details: {ex.Message}");
                return NotFound();
            }
        }

        [HttpGet("Home/SearchAjax")]
        public async Task<IActionResult> SearchAjax(string? location, decimal? minPrice, decimal? maxPrice, string? type)
        {
            List<PropertyModel> properties = new List<PropertyModel>();
            try
            {
                var queryParams = new List<string>();
                if (!string.IsNullOrEmpty(location)) queryParams.Add($"location={Uri.EscapeDataString(location)}");
                if (minPrice.HasValue) queryParams.Add($"minPrice={minPrice}");
                if (maxPrice.HasValue) queryParams.Add($"maxPrice={maxPrice}");
                if (!string.IsNullOrEmpty(type)) queryParams.Add($"type={Uri.EscapeDataString(type)}");

                string queryString = queryParams.Any() ? "?" + string.Join("&", queryParams) : "";

                // We are allowing AllowAnonymous on the API side for Search, or we can optionally pass the token if needed.
                HttpResponseMessage response = await _client.GetAsync($"{apiUrl}/search{queryString}");

                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    properties = JsonConvert.DeserializeObject<List<PropertyModel>>(jsonResponse);
                    return PartialView("_PropertyGrid", properties); // We will create a partial view
                }
            }
            catch (System.Exception ex)
            {
                System.Console.WriteLine($"Error fetching data: {ex.Message}");
            }
            
            return PartialView("_PropertyGrid", properties);
        }

    }
}
