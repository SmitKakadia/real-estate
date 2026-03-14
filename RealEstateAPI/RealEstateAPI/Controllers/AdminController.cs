using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstateAPI.Data;
using RealEstateAPI.Model;

namespace RealEstateAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AdminController : ControllerBase
    {
        private readonly AdminRepository _adminRepo;
        private readonly PropertyRepository _propertyRepo;
        private readonly FeedbackRepository _feedbackRepo;

        public AdminController(AdminRepository adminRepo, PropertyRepository propertyRepo, FeedbackRepository feedbackRepo)
        {
            _adminRepo = adminRepo;
            _propertyRepo = propertyRepo;
            _feedbackRepo = feedbackRepo;
        }

        // GET api/Admin/stats
        [HttpGet("stats")]
        public IActionResult GetStats() => Ok(_adminRepo.GetAdminStats());

        // GET api/Admin/activity
        [HttpGet("activity")]
        public IActionResult GetActivity()
        {
            var (props, users) = _adminRepo.GetRecentActivity();
            return Ok(new { recentProperties = props, recentUsers = users });
        }

        // GET api/Admin/users
        [HttpGet("users")]
        public IActionResult GetUsers() => Ok(_adminRepo.GetAllUsers());

        // GET api/Admin/all-properties
        [HttpGet("all-properties")]
        public IActionResult GetAllProperties() => Ok(_adminRepo.GetAllPropertiesAdmin());

        // GET api/Admin/feedback
        [HttpGet("feedback")]
        public IActionResult GetFeedback() => Ok(_adminRepo.GetAllFeedbackAdmin());

        // PUT api/Admin/update-role
        [HttpPut("update-role")]
        public IActionResult UpdateRole([FromBody] UpdateRoleRequest request)
        {
            if (_adminRepo.UpdateUserRole(request.UserID, request.NewRole))
                return Ok(new { message = "Role updated successfully." });
            return StatusCode(500, "Failed to update role.");
        }

        // PUT api/Admin/approve/{id}
        [HttpPut("approve/{propertyID}")]
        public IActionResult ApproveProperty(int propertyID)
        {
            if (_propertyRepo.ApproveProperty(propertyID))
                return Ok(new { message = "Property approved." });
            return StatusCode(500, "Failed to approve.");
        }

        // PUT api/Admin/reject/{id}
        [HttpPut("reject/{propertyID}")]
        public IActionResult RejectProperty(int propertyID, [FromBody] string reason)
        {
            if (_propertyRepo.RejectProperty(propertyID, reason))
                return Ok(new { message = "Property rejected." });
            return StatusCode(500, "Failed to reject.");
        }

        // DELETE api/Admin/feedback/{id}
        [HttpDelete("feedback/{feedbackID}")]
        public IActionResult DeleteFeedback(int feedbackID)
        {
            if (_feedbackRepo.DeleteFeedback(feedbackID))
                return Ok(new { message = "Review deleted." });
            return StatusCode(500, "Failed to delete review.");
        }

        // DELETE api/Admin/property/{id}
        [HttpDelete("property/{propertyID}")]
        public IActionResult DeleteProperty(int propertyID)
        {
            if (_propertyRepo.DeleteProperty(propertyID))
                return Ok(new { message = "Property deleted." });
            return StatusCode(500, "Failed to delete property.");
        }
    }
}
