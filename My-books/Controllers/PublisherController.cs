using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using My_books.Data.Models;
using My_books.Data.Services;
using My_books.Data.ViewModels;
using My_books.Data.ViewModels.Authentication;
using My_books.Exceptions;
using System.Reflection.Metadata;
using System.Security.Claims;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace My_books.Controllers
{
    //[Authorize(Roles =UserRoles.Publisher+","+UserRoles.Admin)]
    [Route("api/[controller]")]
    [ApiController]
 
    public class PublisherController : ControllerBase
    {
        private PublisherService _publisherService;
        private readonly ILogger<PublisherController> _logger;
        private readonly IAuthorizationService _authorizationService;



        #region [-Ctor-]
        public PublisherController(PublisherService publisherService, ILogger<PublisherController> logger, IAuthorizationService authorizationService)
        {
            _publisherService = publisherService;
            _logger = logger;
            _authorizationService = authorizationService;
        }
        #endregion


        #region [-Get-Publisher-Books-With-Authors-By-Id-]
        [HttpGet("get-publisher-books-with-authors/{id}")]
        public IActionResult GetPublisherData(int id)
        {
            var _response = _publisherService.GetPublisherData(id);
            return Ok(_response);
        }
        #endregion

        #region [-Get-Publisher-By-Id-]
        [HttpGet("get-publisher-by-id/{id}")]
        public IActionResult GetPublisherById(int id)
        {

            var _response = _publisherService.GetPublisherById(id);
            if (_response != null)
            {
                return Ok(_response);
            }
            else
            {
                return NotFound();

            }
        }
        #endregion

        #region [-Get-All-Publisher-]
        [HttpGet("get-all-publisher")]
        public IActionResult GetAllublishers(string sortBy, string searchString, int pageNumber)
        {

            try
            {
                _logger.LogInformation("This is just a log in GetAllpublishers()");
                var allpublishers = _publisherService.GetAllPublishers(sortBy, searchString, pageNumber);
                return Ok(allpublishers);
            }
            catch (Exception ex)
            {
                return BadRequest("Sorry we could not load the publishers");
            }

        }
        #endregion

        #region [-Post-Publisher-]
        [Authorize(Policy ="PublisherWrite")]
        [HttpPost("add-publisher")]
        public async Task<IActionResult> AddPublisher([FromBody] PublisherVM publisher)
        {
            try // try: code that might throw an error
            {
                var requirement = new OperationRequirement("Write");
                var result = await _authorizationService.AuthorizeAsync(User, publisher, requirement);
                if (!result.Succeeded)  Forbid(); 
                //{ return Forbid();}
                var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

                var addedPublisher = _publisherService.AddPublisher(publisher, userId);

                //var newPublisher = _publisherService.AddPublisher(publisher);
                return Created(nameof(AddPublisher), addedPublisher);

            }
            catch (PublisherNameException ex) //handle the error
            {
                return BadRequest($"{ex.Message}, Publisher name: {ex.PublisherName}");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.InnerException?.Message ??ex.Message);
            }
            finally// finally: always executes

            {
            }
        }
        #endregion

        #region [-Put-Publisher-By-Id-]
        [HttpPut("update-publisher-by-id/{id}")]
        public IActionResult PutPublisherById(int id, [FromBody] PublisherVM publisher)
        {
            var updatedPublisher = _publisherService.UpdatePublisherById(id, publisher);
            return Ok(updatedPublisher);
        }
        #endregion

        #region [-Delete-Publisher-By-Id-]
        //[Authorize(Policy = "PublisherDelete")]
        [HttpDelete("delete-publisher-by-id/{id}")]
        public async Task<IActionResult> DeletePublisherById(int id)
        {
            try
            {
                var publisher = _publisherService.GetPublisherById(id);
                if (publisher == null)
                    return NotFound();

                // Resource-based authorization
                var requirement = new OperationRequirement("Delete");
                var result = await _authorizationService.AuthorizeAsync(User, publisher, requirement);

                if (!result.Succeeded)
                    return Forbid(); // 403

                _publisherService.DeletePublisherById(id);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        #endregion
    }
}
