using Microsoft.AspNetCore.Mvc;
using My_books.Data.Models;
using My_books.Data.Services;
using My_books.Data.ViewModels;

namespace My_books.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PublisherController : ControllerBase
    {
        public PublisherService _publisherService;

        public PublisherController(PublisherService publisherService)
        {
            _publisherService = publisherService;
        }

        [HttpGet("get-publisher-by-id/{id}")]

        public IActionResult GetPublisherById(int id)
        {
            var publisher = _publisherService.GetPublisherById(id);
            return Ok(publisher);
        }

        #region [-Get-All-publisher-]
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
            _publisherService.AddPublisher(publisher);
          
            return Ok();
        }
        #endregion

        #region [-Put-publisher-]
        [HttpPut("update-publisher-by-id/{id}")]
        public IActionResult PutPublisherById(int id, [FromBody] PublisherVM publisher)
        {
            var updatedPublisher = _publisherService.UpdatePublisherById(id, publisher);
            return Ok(updatedPublisher);
        }
        #endregion

        [HttpDelete("delete-publisher-by-id/{id}")]
        public IActionResult DeletePublisherById(int id)
        {
            _publisherService.DeletePublisherById(id);
            return Ok();

        }

    }
}
