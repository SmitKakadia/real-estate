using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RealEstateAPI.Data;
using RealEstateAPI.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RealEstateAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PropertyController : ControllerBase
    {
        private readonly PropertyRepository _propertyRepository;
        private readonly CloudinaryService _cloudinaryService;

        public PropertyController(PropertyRepository propertyRepository, CloudinaryService cloudinaryService)
        {
            _propertyRepository = propertyRepository;
            _cloudinaryService = cloudinaryService;
        }

        #region GetAllProperties
        [HttpGet("AllProperties")]
        public IActionResult GetAllPropertiess()
        {
            var properties = _propertyRepository.SelectAllPropertys();
            return Ok(properties);
        }
        #endregion

        #region GetAllPropertiesBypending
        [HttpGet]
        public IActionResult GetAllProperties()
        {
            var properties = _propertyRepository.SelectAllProperty();
            return Ok(properties);
        }
        #endregion

        #region GetPropertyByID
        [HttpGet("{propertyID}")]
        public IActionResult GetPropertyByID(int propertyID)
        {
            var property = _propertyRepository.SelectPropertyByID(propertyID);
            if (property == null)
            {
                return NotFound("Property not found.");
            }
            return Ok(property);
        }
        #endregion

        #region InsertProperty
        [HttpPost]
        public async Task<IActionResult> InsertProperty([FromForm] PropertyDto propertyDto)
        {
            if (propertyDto == null || propertyDto.Image == null)
            {
                return BadRequest("Invalid property data.");
            }

            // Upload image to Cloudinary
            var imageUrl = await _cloudinaryService.UploadImageAsync(propertyDto.Image);
            if (string.IsNullOrEmpty(imageUrl))
            {
                return StatusCode(500, "Image upload failed.");
            }

            var property = new PropertyModel
            {
                Title = propertyDto.Title,
                Location = propertyDto.Location,
                Description = propertyDto.Description,
                Price = propertyDto.Price,
                Type = propertyDto.Type,
                Status = propertyDto.Status,
                UserID = propertyDto.UserID,
                ImageUrl = imageUrl
            };

            if (propertyDto.AdditionalImages != null && propertyDto.AdditionalImages.Count > 0)
            {
                foreach (var file in propertyDto.AdditionalImages)
                {
                    var addImgUrl = await _cloudinaryService.UploadImageAsync(file);
                    if (!string.IsNullOrEmpty(addImgUrl))
                    {
                        property.AdditionalImages.Add(addImgUrl);
                    }
                }
            }

            var isInserted = _propertyRepository.InsertProperty(property);
            if (!isInserted)
            {
                return BadRequest("Failed to insert property.");
            }

            return CreatedAtAction(nameof(GetPropertyByID), new { propertyID = property.PropertyID }, property);
        }
        #endregion

        #region UpdateProperty

        [HttpPut("{propertyID}")]
        public async Task<IActionResult> UpdateProperty(int propertyID, [FromForm] PropertyDto propertyDto)
        {
            if (propertyID != propertyDto.PropertyID)
            {
                return BadRequest("Property ID mismatch.");
            }

            var existingProperty = _propertyRepository.SelectPropertyByID(propertyID);
            if (existingProperty == null)
            {
                return NotFound("Property not found.");
            }

            string imageUrl = existingProperty.ImageUrl;
            if (propertyDto.Image != null)
            {
                // Upload new image to Cloudinary
                imageUrl = await _cloudinaryService.UploadImageAsync(propertyDto.Image);
                if (string.IsNullOrEmpty(imageUrl))
                {
                    return StatusCode(500, "Image upload failed.");
                }
            }

            var updatedProperty = new PropertyModel
            {
                PropertyID = propertyID,
                Title = propertyDto.Title,
                Location = propertyDto.Location,
                Description = propertyDto.Description,
                Price = propertyDto.Price,
                Type = propertyDto.Type,
                Status = propertyDto.Status,
                UserID = propertyDto.UserID,
                ImageUrl = imageUrl
            };

            var isUpdated = _propertyRepository.UpdateProperty(updatedProperty);
            if (!isUpdated)
            {
                return BadRequest("Failed to update property.");
            }

            return Ok(updatedProperty);
        }
        #endregion

        #region DeleteProperty
        [HttpDelete("{propertyID}")]
        public IActionResult DeleteProperty(int propertyID)
        {
            var isDeleted = _propertyRepository.DeleteProperty(propertyID);
            if (!isDeleted)
            {
                return BadRequest("Failed to delete property.");
            }
            return NoContent();
        }
        #endregion

        #region UpdatePropertyStatusToSold
        [HttpPut("update-status/{propertyID}/{userID}")]
        public IActionResult UpdatePropertyStatusToSold(int propertyID, int userID)
        {
            var isUpdated = _propertyRepository.UpdatePropertyStatusToSold(propertyID, userID);
            if (!isUpdated)
            {
                return BadRequest("Failed to update property status to sold.");
            }
            return Ok("Property status updated to sold successfully.");
        }
        #endregion



        #region GetPropertiesForBuyer
        [HttpGet("buyer/{userID}")]
        public IActionResult GetPropertiesForBuyer(int userID)
        {
           
            var buyerProperties = _propertyRepository.SelectPropertiesByBuyer(userID);

           
            if (buyerProperties == null || buyerProperties.Count == 0)
            {
                return Ok("This buyer did not purchase any property.");
            }

            return Ok(buyerProperties);
        }
        #endregion

        #region SearchProperties
        [HttpGet("search")]
        [AllowAnonymous]
        public IActionResult SearchProperties([FromQuery] string? location, [FromQuery] decimal? minPrice, [FromQuery] decimal? maxPrice, [FromQuery] string? type)
        {
            var properties = _propertyRepository.SearchProperties(location, minPrice, maxPrice, type);
            return Ok(properties);
        }
        #endregion

    }
}
