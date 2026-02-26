using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RealEstateAPI.Data;
using RealEstateAPI.Model;

namespace RealEstateAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class FeedbackController : ControllerBase
    {
        private readonly FeedbackRepository _feedbackRepository;

        public FeedbackController(FeedbackRepository feedbackRepository)
        {
            _feedbackRepository = feedbackRepository;
        }


        #region GetAllFeedback
        [HttpGet]

        public IActionResult GetAllFeedback()
        {
            var feedbacks = _feedbackRepository.SelectAllFeedback();
            return Ok(feedbacks);
        }
        #endregion

        #region GetFeedbackByID

        [HttpGet("{FeedbackID}")]

        public IActionResult GetFeedbackByID(int FeedbackID)
        {
            var feedbacks = _feedbackRepository.SelectFeedbackByID(FeedbackID);
            return Ok(feedbacks);
        }
        #endregion

        #region InsertFeedback

        [HttpPost]
        public IActionResult InsertFeedback([FromBody] FeedbackModel feedback)
        {
            var feedbacks = _feedbackRepository.InsertFeedback(feedback);
            if (!feedbacks)
            {
                return BadRequest();
            }
            return Ok(feedbacks);
        }

        #endregion

        #region UpdateFeedback

        [HttpPut("{feedbackID}")]
        public IActionResult UpdateUser(int feedbackID, FeedbackModel feedback)
        {
            // Ensure the UserID from URL matches the UserID in the model
            if (feedbackID != feedback.FeedbackID)
            {
                return BadRequest("FeedbackID mismatch.");
            }

            var feedbacks = _feedbackRepository.UpdateFeedback(feedback);
            if (!feedbacks)
            {
                return BadRequest("Failed to update feedback.");
            }

            return Ok(feedbacks);
        }
        #endregion

        #region DeleteFeedback

        [HttpDelete("{FeedbackID}")]
        public IActionResult DeleteFeedback(int FeedbackID)
        {
            var feedbacks = _feedbackRepository.DeleteFeedback(FeedbackID);
            if (!feedbacks)
            {
                return BadRequest();
            }
            return NoContent();
        }
        #endregion

        #region GetFeedbackByPropertyID
        // GET: api/Feedback/property/5
        [HttpGet("property/{propertyID}")]
        public IActionResult GetFeedbackByPropertyID(int propertyID)
        {
            try
            {
                var feedbacks = _feedbackRepository.SelectFeedbackByPropertyID(propertyID);
                return Ok(feedbacks);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    $"Internal server error: {ex.Message}");
            }
        }
        #endregion
    }
}
