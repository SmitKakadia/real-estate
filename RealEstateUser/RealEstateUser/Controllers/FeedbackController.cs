using Microsoft.AspNetCore.Mvc;
using RealEstateUser.Models;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System;
using System.Net.Http.Json;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace RealEstateUser.Controllers
{
    public class FeedbackController : Controller
    {
        private readonly HttpClient _client;
        private readonly string apiUrl = "http://localhost:5127/api/Feedback";

        public FeedbackController(IHttpClientFactory httpClientFactory)
        {
            _client = httpClientFactory.CreateClient();
        }

        // GET: Display Feedback Form (if needed)
        public IActionResult Index()
        {
            var model = new FeedbackModel();
            return View("Feedback", model);
        }

        // POST: /Feedback/SubmitFeedback
        [HttpPost]
        public async Task<IActionResult> SubmitFeedback(FeedbackModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid feedback data.");
            }

            try
            {
                // Retrieve JWT token from session
                string token = HttpContext.Session.GetString("AuthToken");
                if (string.IsNullOrEmpty(token))
                {
                    return Unauthorized("User is not authenticated.");
                }
                _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                
                if (model.UserID == 0)
                {
                    string userId = HttpContext.Session.GetString("UserID");
                    if (!string.IsNullOrEmpty(userId))
                    {
                        model.UserID = int.Parse(userId);
                    }
                }

                
                HttpResponseMessage response = await _client.PostAsJsonAsync(apiUrl, model);
                if (response.IsSuccessStatusCode)
                {
                    return Ok("Feedback submitted successfully.");
                }
                else
                {
                    return BadRequest("Failed to submit feedback.");
                }
            }
            catch (Exception ex)
            {
                return BadRequest("Error: " + ex.Message);
            }
        }

        // GET: /Feedback/GetFeedbackByPropertyID/{id}
        [HttpGet]
        [Produces("application/json")]
        public async Task<IActionResult> GetFeedbackByPropertyID(int id)
        {
            List<FeedbackModel> feedbackList = new List<FeedbackModel>();

            try
            {
                // Retrieve JWT token from session if available
                string token = HttpContext.Session.GetString("AuthToken");
                if (!string.IsNullOrEmpty(token))
                {
                    _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                }

               
                HttpResponseMessage response = await _client.GetAsync($"{apiUrl}/property/{id}");
                if (response.IsSuccessStatusCode)
                {
                    feedbackList = await response.Content.ReadFromJsonAsync<List<FeedbackModel>>();
                }
                else
                {
                    return BadRequest("Failed to retrieve feedback.");
                }
            }
            catch (Exception ex)
            {
                return BadRequest("Error: " + ex.Message);
            }

            return Ok(feedbackList);
        }
    }
}
