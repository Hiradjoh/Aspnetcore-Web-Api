using Microsoft.AspNetCore.Mvc;
using My_books.Data.Services;
using My_books.Data.ViewModels;

namespace My_books.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        public BookService _bookService;

        #region [-Ctor-]
        public BooksController(BookService bookService)
        {
            _bookService = bookService;
        }
        #endregion

        #region [-GetBookById-]
        [HttpGet("get-book-by-id/{id}")]
        public ActionResult<BookAuthorVM> GetBookById(int id)
        {
            var book = _bookService.GetBookById(id);
            return Ok(book);
        }
        #endregion

        #region [-Get-All-Books-]
        [HttpGet("get-all-books")]
        public IActionResult GetAllBooks()
        {
            var allbooks = _bookService.GetAllBooks();
            return Ok(allbooks);
        }
        #endregion

        #region [-Post-Book-]
        [HttpPost("add-book-with-authors")]
        public IActionResult AddBook([FromBody] BookVM book)
        {
            _bookService.AddBookWithAuthors(book);
            return Ok();
        }
        #endregion

        #region [-Put-Book-]
        [HttpPut("update-book-by-id/{id}")]
        public IActionResult PutBookById(int id, [FromBody] BookVM book)
        {
            var updatedBook = _bookService.UpdateBookById(id, book);
            return Ok(updatedBook);
        }
        #endregion

        #region [-DeleteBookById-]
        [HttpDelete("delete-book-by-id/{id}")]
        public IActionResult DeleteBookById(int id)
        {
            _bookService.DeleteById(id);
            return Ok();

        }
        #endregion

    }
}
