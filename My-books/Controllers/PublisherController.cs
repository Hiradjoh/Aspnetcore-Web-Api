using Microsoft.AspNetCore.Mvc;
using My_books.Data.Models;
using My_books.Data.Services;
using My_books.Data.ViewModels;
using My_books.Exceptions;

namespace My_books.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PublisherController : ControllerBase
    {
        public PublisherService _publisherService;

        #region [-Ctor-]
        public PublisherController(PublisherService publisherService)
        {
            _publisherService = publisherService;
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
        public IActionResult GetAllpublishers()
        {
            var allpublishers = _publisherService.GetAllPublishers();
            return Ok(allpublishers);
        }
        #endregion

        #region [-Post-Publisher-]
        [HttpPost("add-publisher")]
        public IActionResult AddPublisher([FromBody] PublisherVM publisher)
        {
            try
            {
                var newPublisher = _publisherService.AddPublisher(publisher);
                return Created(nameof(AddPublisher), newPublisher);
            }
            catch (PublisherNameException ex)
            {
                return BadRequest($"{ex.Message}, Publisher name: {ex.PublisherName}");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
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
        [HttpDelete("delete-publisher-by-id/{id}")]
        public IActionResult DeletePublisherById(int id)
        {
            try
            {
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
