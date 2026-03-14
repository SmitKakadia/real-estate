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
        [HttpGet]
        public async Task<IActionResult> Index(int? resellPropertyId, int? editPropertyId)
        {
            ViewBag.ActiveMenu = "Sell";
            ViewBag.IsResell = false; // Default: normal listing or edit
            PropertyDto model = new PropertyDto();

            if (resellPropertyId.HasValue)
            {
                string token = HttpContext.Session.GetString("AuthToken");
                if (!string.IsNullOrEmpty(token))
                {
                    _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                    HttpResponseMessage response = await _client.GetAsync($"{apiUrl}/{resellPropertyId.Value}");

                    if (response.IsSuccessStatusCode)
                    {
                        var jsonResponse = await response.Content.ReadAsStringAsync();
                        var property = JsonConvert.DeserializeObject<PropertyModel>(jsonResponse);
                        
                        string currentUserIdStr = HttpContext.Session.GetString("UserID");
                        int.TryParse(currentUserIdStr, out int currentUserId);

                        if (property != null && property.BuyerID == currentUserId)
                        {
                            ViewBag.IsResell = true; // This is a buyer reselling a purchased property
                            model.PropertyID = resellPropertyId.Value;
                            model.Title = property.Title;
                            model.Description = property.Description;
                            model.Location = property.Location;
                            model.Price = property.Price;
                            model.Type = "Sell";
                            model.ExistingImageUrl = property.ImageUrl;
                            if (property.AdditionalImages != null && property.AdditionalImages.Count > 0)
                            {
                                model.ExistingAdditionalImages = property.AdditionalImages;
                            }
                        }
                        else 
                        {
                            TempData["ErrorMessage"] = "You can only resell properties that you have purchased.";
                            return RedirectToAction("MyProperties", "Property");
                        }
                    }
                }
            }
            else if (editPropertyId.HasValue)
            {
                string token = HttpContext.Session.GetString("AuthToken");
                if (!string.IsNullOrEmpty(token))
                {
                    _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                    HttpResponseMessage response = await _client.GetAsync($"{apiUrl}/{editPropertyId.Value}");

                    if (response.IsSuccessStatusCode)
                    {
                        var jsonResponse = await response.Content.ReadAsStringAsync();
                        var property = JsonConvert.DeserializeObject<PropertyModel>(jsonResponse);
                        
                        string currentUserIdStr = HttpContext.Session.GetString("UserID");
                        int.TryParse(currentUserIdStr, out int currentUserId);

                        if (property != null && property.UserID == currentUserId)
                        {
                            ViewBag.IsResell = false; // This is the original seller editing/resubmitting
                            model.PropertyID = editPropertyId.Value;
                            model.Title = property.Title;
                            model.Description = property.Description;
                            model.Location = property.Location;
                            model.Price = property.Price;
                            model.Type = property.Type;
                            model.ExistingImageUrl = property.ImageUrl;
                            model.RejectionReason = property.RejectionReason;
                            if (property.AdditionalImages != null && property.AdditionalImages.Count > 0)
                            {
                                model.ExistingAdditionalImages = property.AdditionalImages;
                            }
                        }
                        else 
                        {
                            TempData["ErrorMessage"] = "You can only edit your own properties.";
                            return RedirectToAction("MyProperties", "Property");
                        }
                    }
                }
            }

            return View(model);
        }

        // POST: Submit new property for sale
        [HttpPost]
        public async Task<IActionResult> Index(PropertyDto model)
        {
            // Validate programmatically handled fields
            ModelState.Remove("UserID");
            if (!string.IsNullOrEmpty(model.ExistingImageUrl))
            {
                ModelState.Remove("ImageUrl");
            }
            ModelState.Remove("AdditionalImages");

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                ViewBag.ErrorMessage = "Validation Error: " + string.Join(" | ", errors);
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
                    content.Add(new StringContent(model.PropertyID?.ToString() ?? "0"), "PropertyID");

                    if (!string.IsNullOrEmpty(model.ExistingImageUrl))
                    {
                        content.Add(new StringContent(model.ExistingImageUrl), "ImageUrl");
                    }

                    if (model.ImageUrl != null && model.ImageUrl.Length > 0)
                    {
                        var fileStream = model.ImageUrl.OpenReadStream();
                        var streamContent = new StreamContent(fileStream);
                        streamContent.Headers.ContentType = new MediaTypeHeaderValue(model.ImageUrl.ContentType);
                        content.Add(streamContent, "Image", model.ImageUrl.FileName);
                    }

                    if (model.ExistingAdditionalImages != null && model.ExistingAdditionalImages.Count > 0)
                    {
                        foreach (var imgStr in model.ExistingAdditionalImages)
                        {
                            content.Add(new StringContent(imgStr), "ExistingAdditionalImages");
                        }
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

                    bool isUpdate = false;
                    if (model.PropertyID.HasValue && model.PropertyID.Value > 0)
                    {
                        var checkResp = await _client.GetAsync($"{apiUrl}/{model.PropertyID.Value}");
                        if (checkResp.IsSuccessStatusCode)
                        {
                            var checkJson = await checkResp.Content.ReadAsStringAsync();
                            var existingProp = Newtonsoft.Json.JsonConvert.DeserializeObject<PropertyModel>(checkJson);
                            
                            // If I am the creator, it's a RESUBMIT (Update)
                            // If I am the buyer, it's a RESELL (New Post + Delete Old)
                            if (existingProp != null && existingProp.UserID == model.UserID)
                            {
                                isUpdate = true;
                            }
                            else if (existingProp == null || existingProp.BuyerID != model.UserID)
                            {
                                ViewBag.ErrorMessage = "You can only resell or edit properties that you own.";
                                return View(model);
                            }
                        }
                    }

                    HttpResponseMessage response;
                    if (isUpdate)
                    {
                        // RESUBMIT/EDIT flow
                        response = await _client.PutAsync($"{apiUrl}/{model.PropertyID.Value}", content);
                    }
                    else
                    {
                        // NEW LISTING / RESELL flow
                        response = await _client.PostAsync(apiUrl, content);
                    }

                    if (response.IsSuccessStatusCode)
                    {
                        // If this was a RESELL (not an update), DELETE the old property reference
                        if (!isUpdate && model.PropertyID.HasValue && model.PropertyID.Value > 0)
                        {
                            try { await _client.DeleteAsync($"{apiUrl}/{model.PropertyID.Value}"); } catch { }
                        }
                        return RedirectToAction("MyProperties", "Property");
                    }
                    else
                    {
                        var errorMsg = await response.Content.ReadAsStringAsync();
                        ViewBag.ErrorMessage = $"Failed to process request. API responded: {response.StatusCode} - {errorMsg}";
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
