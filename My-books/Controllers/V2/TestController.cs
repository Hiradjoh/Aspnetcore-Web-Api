using Microsoft.AspNetCore.Mvc;

namespace My_books.Controllers.V2
{
    //[ApiVersion("2.0")]
    //[Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class TestController : Controller
    {
        [HttpGet("get-test-data")]
        public IActionResult Get()
        {
            return Ok("this is test controller V2 ");
        }
    }
}
