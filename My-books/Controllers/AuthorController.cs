using Microsoft.AspNetCore.Mvc;
using My_books.Data.Services;
using My_books.Data.ViewModels;

namespace My_books.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class AuthorController : ControllerBase
    {
        public AuthorService _authorService;

        #region [-Ctor-]
        public AuthorController(AuthorService authorService)
        {
            _authorService = authorService;
        } 
        #endregion

        #region [-Get-Author-By-Id-]
        [HttpGet("get-author-by-id/{id}")]
        public IActionResult GetAuthorById(int id)
        {
            var author = _authorService.GetAuthorById(id);
            return Ok(author);
        }
        #endregion

        #region [-Get-All-Author-]
        [HttpGet("get-all-author")]
        public IActionResult GetAllAuthor()
        {
            var allauthor = _authorService.GetAllAuthors();
            return Ok(allauthor);
        }
        #endregion

        #region [-Post-Author-]
        [HttpPost("add-author")]
        public IActionResult AddAuthor([FromBody] AuthorVM author)
        {
            _authorService.AddAuthor(author);
            return Ok();
        }
        #endregion

        #region [-Put-Author-]
        [HttpPut("update-author-by-id/{id}")]
        public IActionResult PutAuthorById(int id, [FromBody] AuthorVM author)
        {
            var updatedAuthor = _authorService.UpdateAuthorById(id, author);
            return Ok(updatedAuthor);
        }
        #endregion

        #region [-Delete-Author-By-Id-]
        [HttpDelete("delete-author-by-id/{id}")]
        public IActionResult DeleteAuthorById(int id)
        {
            _authorService.DeleteById(id);
            return Ok();
        }
        #endregion

        #region [-Get-Author-With-Books-By-Id-]
        [HttpGet("get-author-with-books-by-id/{id}")]
        public IActionResult GetAuthorWithBooks(int id)
        {
            var respons = _authorService.GetAuthorWithBooks(id);
            return Ok(respons);
        }
        #endregion

    }
}


