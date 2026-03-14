using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RealEstateUser.Models;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.AspNetCore.Http;
using System;
using System.Net.Http.Json;

namespace RealEstateUser.Controllers
{
    [Route("Property")]
    public class PropertyController : Controller
    {
        private readonly HttpClient _client;
        private readonly string apiUrl = "http://localhost:5127/api/Property"; 

        public PropertyController(IHttpClientFactory httpClientFactory)
        {
            _client = httpClientFactory.CreateClient();
        }

      

        [HttpGet("MyProperties")]
        public async Task<IActionResult> MyProperties()
        {
            ViewBag.ActiveMenu = "MyProperties";
            List<PropertyModel> userProperties = new List<PropertyModel>();

            try
            {
                // Retrieve token and user id from session
                string token = HttpContext.Session.GetString("AuthToken");
                string userIdString = HttpContext.Session.GetString("UserID");

                if (string.IsNullOrEmpty(token))
                {
                    return RedirectToAction("Login", "Auth");
                }

                if (!int.TryParse(userIdString, out int userId))
                {
                    ViewBag.ErrorMessage = "User ID not found or invalid.";
                    return View(userProperties);
                }

                
                _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

               
                HttpResponseMessage response = await _client.GetAsync($"{apiUrl}/buyer/{userId}");

                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    userProperties = JsonConvert.DeserializeObject<List<PropertyModel>>(jsonResponse) ?? new List<PropertyModel>();
                }
                else
                {
                    ViewBag.ErrorMessage = "Failed to retrieve properties.";
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching properties: {ex.Message}");
                ViewBag.ErrorMessage = "Error fetching your properties.";
            }

            return View(userProperties);
        }

        
        [HttpGet("Details/{id}")]
        public async Task<IActionResult> Details(int id)
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
                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    var property = JsonConvert.DeserializeObject<PropertyModel>(jsonResponse);
                    return View(property);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching property details: {ex.Message}");
            }

            return BadRequest("Error retrieving property details.");
        }

        // POST: /Property/BuyPropertyAjax
        [HttpPost("BuyPropertyAjax")]
        public async Task<IActionResult> BuyPropertyAjax(int propertyID)
        {
            try
            {
               
                string token = HttpContext.Session.GetString("AuthToken");
                if (string.IsNullOrEmpty(token))
                {
                    return Json(new { success = false, message = "User is not authenticated." });
                }

                
                _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                // Retrieve user id from token claims
                var userIdClaim = User.FindFirst("UserID");
                int userID = 0;
                if (userIdClaim != null)
                {
                    userID = int.Parse(userIdClaim.Value);
                }
                else
                {
                    // Fallback: get user id from session if not available in token
                    string userIdString = HttpContext.Session.GetString("UserID");
                    if (string.IsNullOrEmpty(userIdString))
                    {
                        return Json(new { success = false, message = "User ID not found." });
                    }
                    userID = int.Parse(userIdString);
                }

                // Verify property ownership and status before calling API
                var propResponse = await _client.GetAsync($"{apiUrl}/{propertyID}");
                if (propResponse.IsSuccessStatusCode)
                {
                    var propJson = await propResponse.Content.ReadAsStringAsync();
                    var property = JsonConvert.DeserializeObject<PropertyModel>(propJson);
                    if (property != null)
                    {
                        if (property.UserID == userID)
                        {
                            return Json(new { success = false, message = "You cannot buy your own property." });
                        }
                        if (property.Status != null && property.Status.ToLower().Contains("sold"))
                        {
                            return Json(new { success = false, message = "This property is already sold." });
                        }
                    }
                }
                var response = await _client.PutAsync($"{apiUrl}/update-status/{propertyID}/{userID}", null);
                if (response.IsSuccessStatusCode)
                {
                    return Json(new
                    {
                        success = true,
                        message = "Property status updated to sold successfully.",
                    });
                }
                else
                {
                    string errorMessage = await response.Content.ReadAsStringAsync();
                    return Json(new { success = false, message = $"Failed to update property status. {errorMessage}" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error: {ex.Message}" });
            }
        }

        [HttpPost("CancelLeaseAjax")]
        public async Task<IActionResult> CancelLeaseAjax(int propertyID)
        {
            try
            {
                string token = HttpContext.Session.GetString("AuthToken");
                string userIdString = HttpContext.Session.GetString("UserID");

                if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(userIdString))
                {
                    return Json(new { success = false, message = "User is not authenticated." });
                }

                _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var response = await _client.PutAsync($"{apiUrl}/cancel-lease/{propertyID}/{userIdString}", null);

                if (response.IsSuccessStatusCode)
                {
                    return Json(new { success = true, message = "Lease cancelled successfully." });
                }
                return Json(new { success = false, message = "Failed to cancel lease." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error: {ex.Message}" });
            }
        }

        // DELETE: /Property/DeleteProperty/5
        [HttpDelete("DeleteProperty/{propertyId}")]
        public async Task<IActionResult> DeleteProperty(int propertyId)
        {
            try
            {
                string token = HttpContext.Session.GetString("AuthToken");
                if (string.IsNullOrEmpty(token))
                {
                    return Json(new { success = false, message = "User is not authenticated." });
                }

                _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

               
              

                HttpResponseMessage response = await _client.DeleteAsync($"{apiUrl}/{propertyId}");
                if (response.IsSuccessStatusCode)
                {
                    return Json(new { success = true, message = "Property deleted successfully." });
                }
                else
                {
                    string errorMessage = await response.Content.ReadAsStringAsync();
                    return Json(new { success = false, message = $"Failed to delete property: {errorMessage}" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error: {ex.Message}" });
            }
        }

        [HttpPost("ToggleFavoriteAjax")]
        public async Task<IActionResult> ToggleFavoriteAjax(int propertyID)
        {
            string token = HttpContext.Session.GetString("AuthToken");
            string userIdString = HttpContext.Session.GetString("UserID");
            if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(userIdString))
            {
                return Json(new { success = false, message = "You must be logged in to favorite properties." });
            }

            int userID = int.Parse(userIdString);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            HttpResponseMessage response = await _client.PostAsync($"{apiUrl}/toggle-favorite/{userID}/{propertyID}", null);
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                dynamic json = JsonConvert.DeserializeObject(content);
                return Json(new { success = true, action = json.message });
            }
            return Json(new { success = false, message = "Failed to update favorites." });
        }

        [HttpGet("SavedProperties")]
        public async Task<IActionResult> SavedProperties()
        {
            ViewBag.ActiveMenu = "SavedProperties";
            string token = HttpContext.Session.GetString("AuthToken");
            string userIdString = HttpContext.Session.GetString("UserID");

            if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(userIdString))
            {
                return RedirectToAction("Login", "Auth");
            }

            int userID = int.Parse(userIdString);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            HttpResponseMessage response = await _client.GetAsync($"{apiUrl}/favorites/{userID}");
            List<PropertyModel> favoriteProperties = new List<PropertyModel>();

            if (response.IsSuccessStatusCode)
            {
                var jsonResponse = await response.Content.ReadAsStringAsync();
                favoriteProperties = JsonConvert.DeserializeObject<List<PropertyModel>>(jsonResponse);
            }

            return View(favoriteProperties);
        }
    }
}
