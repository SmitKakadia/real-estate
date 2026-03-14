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
        private readonly string apiUrl = "http://localhost:5127/api/Admin";

        public AdminController(IHttpClientFactory httpClientFactory)
        {
            _client = httpClientFactory.CreateClient();
        }

        private bool IsAdmin()
        {
            var role = HttpContext.Session.GetString("Role");
            var token = HttpContext.Session.GetString("AuthToken");
            return role == "Admin" && !string.IsNullOrEmpty(token);
        }

        private void SetAuthHeader()
        {
            var token = HttpContext.Session.GetString("AuthToken");
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.ActiveMenu = "Admin";
            if (!IsAdmin()) return RedirectToAction("Login", "Auth");

            SetAuthHeader();

            var vm = new AdminDashboardViewModel();

            // Stats
            var statsResp = await _client.GetAsync($"{apiUrl}/stats");
            if (statsResp.IsSuccessStatusCode)
                vm.Stats = JsonConvert.DeserializeObject<AdminStatsModel>(await statsResp.Content.ReadAsStringAsync());

            // Activity
            var actResp = await _client.GetAsync($"{apiUrl}/activity");
            if (actResp.IsSuccessStatusCode)
            {
                dynamic act = JsonConvert.DeserializeObject<dynamic>(await actResp.Content.ReadAsStringAsync());
                vm.RecentProperties = JsonConvert.DeserializeObject<List<ActivityItemModel>>(act.recentProperties.ToString());
                vm.RecentUsers = JsonConvert.DeserializeObject<List<ActivityItemModel>>(act.recentUsers.ToString());
            }

            // Pending Approvals
            var pendingResp = await _client.GetAsync($"{apiUrl.Replace("Admin", "Property")}/unapproved");
            if (pendingResp.IsSuccessStatusCode)
                vm.PendingProperties = JsonConvert.DeserializeObject<List<PropertyModel>>(await pendingResp.Content.ReadAsStringAsync());

            // All Users
            var usersResp = await _client.GetAsync($"{apiUrl}/users");
            if (usersResp.IsSuccessStatusCode)
                vm.AllUsers = JsonConvert.DeserializeObject<List<AdminUserModel>>(await usersResp.Content.ReadAsStringAsync());

            // All Properties
            var propsResp = await _client.GetAsync($"{apiUrl}/all-properties");
            if (propsResp.IsSuccessStatusCode)
                vm.AllProperties = JsonConvert.DeserializeObject<List<AdminPropertyModel>>(await propsResp.Content.ReadAsStringAsync());

            // All Feedback
            var fbResp = await _client.GetAsync($"{apiUrl}/feedback");
            if (fbResp.IsSuccessStatusCode)
                vm.AllFeedback = JsonConvert.DeserializeObject<List<AdminFeedbackModel>>(await fbResp.Content.ReadAsStringAsync());

            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> ApproveProperty(int id)
        {
            if (!IsAdmin()) return Json(new { success = false, message = "Unauthorized" });
            SetAuthHeader();
            var response = await _client.PutAsync($"{apiUrl}/approve/{id}", null);
            if (response.IsSuccessStatusCode)
                return Json(new { success = true, message = "Property approved and is now live!" });
            return Json(new { success = false, message = "Failed to approve property." });
        }

        [HttpPost]
        public async Task<IActionResult> RejectProperty(int id, string reason)
        {
            if (!IsAdmin()) return Json(new { success = false, message = "Unauthorized" });
            SetAuthHeader();
            var payload = new StringContent(
                Newtonsoft.Json.JsonConvert.SerializeObject(reason ?? "No reason provided."),
                System.Text.Encoding.UTF8, "application/json");
            var response = await _client.PutAsync($"{apiUrl}/reject/{id}", payload);
            if (response.IsSuccessStatusCode)
                return Json(new { success = true, message = "Property rejected successfully." });
            return Json(new { success = false, message = "Failed to reject property." });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteProperty(int id)
        {
            if (!IsAdmin()) return Unauthorized();
            SetAuthHeader();
            await _client.DeleteAsync($"{apiUrl}/property/{id}");
            TempData["SuccessMessage"] = "Property deleted.";
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteFeedback(int id)
        {
            if (!IsAdmin()) return Unauthorized();
            SetAuthHeader();
            await _client.DeleteAsync($"{apiUrl}/feedback/{id}");
            TempData["SuccessMessage"] = "Review deleted.";
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> UpdateRole(int userId, string newRole)
        {
            if (!IsAdmin()) return Unauthorized();
            SetAuthHeader();
            var payload = new StringContent(
                Newtonsoft.Json.JsonConvert.SerializeObject(new { UserID = userId, NewRole = newRole }),
                System.Text.Encoding.UTF8, "application/json");
            await _client.PutAsync($"{apiUrl}/update-role", payload);
            TempData["SuccessMessage"] = $"User role updated to {newRole}.";
            return RedirectToAction("Index");
        }
        [HttpPost]
        public async Task<IActionResult> DeleteUser(int userId)
        {
            if (!IsAdmin()) return Unauthorized();
            SetAuthHeader();
            await _client.DeleteAsync($"http://localhost:5127/api/User/{userId}");
            TempData["SuccessMessage"] = "User removed.";
            return RedirectToAction("Index");
        }
    }
}
