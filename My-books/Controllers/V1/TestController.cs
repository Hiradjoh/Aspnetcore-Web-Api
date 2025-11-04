using Microsoft.AspNetCore.Mvc;

namespace My_books.Controllers.V1
{
   [ApiVersion ("1.0")] // query versioning 
    [Route("api/[controller]")]

    //[Route("api/v{version:apiVersion}/[controller]")] // url versioning 
    [ApiController]
    public class TestController : Controller
    {
        [HttpGet("get-test-data-from-v1 ")]
        public IActionResult Get()
        {
            return Ok("this is test controller V1 ");
        }
         

    }
}
