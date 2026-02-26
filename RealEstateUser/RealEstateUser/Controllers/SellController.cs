using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RealEstateUser.Models;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;

namespace RealEstateUser.Controllers
{
    public class SellController : Controller
    {
        private readonly HttpClient _client;
        private readonly string apiUrl = "http://localhost:5127/api/Property"; 

        public SellController(IHttpClientFactory httpClientFactory)
        {
            _client = httpClientFactory.CreateClient();
        }

        // GET: Display the form for adding a property
        public IActionResult Index()
        {
            ViewBag.ActiveMenu = "Sell";
            return View(new PropertyDto());
        }

        // POST: Submit new property for sale
        [HttpPost]
        public async Task<IActionResult> Index(PropertyDto model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.ErrorMessage = "Please fill in all required fields.";
                return View(model);
            }

            try
            {
                string token = HttpContext.Session.GetString("AuthToken");
                if (string.IsNullOrEmpty(token))
                {
                    return Unauthorized("You need to log in first.");
                }

                _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

               
                model.UserID = int.Parse(HttpContext.Session.GetString("UserID"));
                // We shouldn't override model.Type if the user selected it, so remove: model.Type = "Sell";
                model.Status = "Pending";

                using (var content = new MultipartFormDataContent())
                {
                    content.Add(new StringContent(model.Title), "Title");
                    content.Add(new StringContent(model.Location), "Location");
                    content.Add(new StringContent(model.Description), "Description");
                    content.Add(new StringContent(model.Price.ToString()), "Price");
                    content.Add(new StringContent(model.Type), "Type");
                    content.Add(new StringContent(model.Status), "Status");
                    content.Add(new StringContent(model.UserID.ToString()), "UserID");

                    if (model.ImageUrl != null && model.ImageUrl.Length > 0)
                    {
                        var fileStream = model.ImageUrl.OpenReadStream();
                        var streamContent = new StreamContent(fileStream);
                        streamContent.Headers.ContentType = new MediaTypeHeaderValue(model.ImageUrl.ContentType);
                        content.Add(streamContent, "Image", model.ImageUrl.FileName);
                    }

                    if (model.AdditionalImages != null && model.AdditionalImages.Count > 0)
                    {
                        foreach (var file in model.AdditionalImages)
                        {
                            if (file != null && file.Length > 0)
                            {
                                var fileStream = file.OpenReadStream();
                                var streamContent = new StreamContent(fileStream);
                                streamContent.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType);
                                content.Add(streamContent, "AdditionalImages", file.FileName);
                            }
                        }
                    }

                    HttpResponseMessage response = await _client.PostAsync(apiUrl, content);

                    if (response.IsSuccessStatusCode)
                    {
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        var errorMsg = await response.Content.ReadAsStringAsync();
                        ViewBag.ErrorMessage = $"Failed to add property. API responded: {response.StatusCode} - {errorMsg}";
                    }
                }
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Server error: " + ex.Message;
            }

            return View(model);
        }
    }
}
