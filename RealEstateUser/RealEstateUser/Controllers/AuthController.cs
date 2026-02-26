using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RealEstateUser.Models;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace RealEstateUser.Controllers
{
    public class AuthController : Controller
    {
        private readonly HttpClient _client;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly string apiUrl = "http://localhost:5127/api/User";

        public AuthController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
            _client = _httpClientFactory.CreateClient();
        }

        public IActionResult Login() => View();

        [HttpPost]
        public async Task<IActionResult> Login(LoginModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.ErrorMessage = "Invalid input!";
                return View(model);
            }

            try
            {
                var jsonContent = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
                HttpResponseMessage response = await _client.PostAsync($"{apiUrl}/login", jsonContent);

                if (response.IsSuccessStatusCode)
                {
                    string jsonResponse = await response.Content.ReadAsStringAsync();
                    dynamic loginResponse = JsonConvert.DeserializeObject<dynamic>(jsonResponse);

                    // Retrieve token and user details (including password)
                    string token = loginResponse.token.ToString();
                    string userId = loginResponse.user.userID.ToString();
                    string userName = loginResponse.user.userName.ToString();
                    string role = loginResponse.user.role.ToString();
                    // Assuming your API returns the password (not recommended for production)
                    string password = loginResponse.user.password.ToString();

                    // Set session values
                    HttpContext.Session.SetString("AuthToken", token);
                    HttpContext.Session.SetString("UserID", userId);
                    HttpContext.Session.SetString("UserName", userName);
                    HttpContext.Session.SetString("Role", role);
                    HttpContext.Session.SetString("UserPassword", password);

                    return RedirectToAction("Index", "Home");
                }

                ViewBag.ErrorMessage = "Invalid email or password!";
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Server error: " + ex.Message;
            }

            return View(model);
        }

        public IActionResult Register() => View();

        [HttpPost]
        public async Task<IActionResult> Register(UserModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.ErrorMessage = "Please fill in all required fields.";
                return View(model);
            }

            try
            {
                var response = await _client.PostAsJsonAsync($"{apiUrl}/register", model);
                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("Login");
                }
                
                string errorResponse = await response.Content.ReadAsStringAsync();
                try 
                {
                    dynamic errorObj = JsonConvert.DeserializeObject<dynamic>(errorResponse);
                    ViewBag.ErrorMessage = errorObj.message;
                } 
                catch 
                {
                    ViewBag.ErrorMessage = "Registration failed. Please try again.";
                }
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Server error: " + ex.Message;
            }

            return View(model);
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}
