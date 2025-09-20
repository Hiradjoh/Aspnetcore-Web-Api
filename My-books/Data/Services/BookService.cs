using My_books.Data.Models;
using My_books.Data.ViewModels;
using System.Threading;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace My_books.Data.Services
{
    public class BookService
    {
        private readonly ProjectDbContext _context;

        public BookService(ProjectDbContext context)
        {
            _context = context;
        }
        public List<Book> GetAllBooks()
        {
            return _context.Books.ToList();
        }
        public Book GetBookById(int bookId)
        {
            return _context.Books.FirstOrDefault(n => n.Id == bookId);
        }
       
        public void AddBook(BookVM book)
        {
            var _book = new Book()
            {
                Title = book.Title,
                Description = book.Description,
                IsRead = book.IsRead,
                DateRead = book.IsRead ? book.DateRead.Value : null,
                Rate = book.IsRead ? book.Rate.Value : null,
                Genre = book.Genre,
                Author = book.Author,
                CoverUrl = book.CoverUrl,
                DateAdded = DateTime.Now

            };
            _context.Books.Add(_book);
            _context.SaveChanges();
        }
        #region [-Update-Book-By-Id-]
        public Book UpdateBookById(int bookId, BookVM book)
        {
            var _book = _context.Books.FirstOrDefault(n => n.Id == bookId);
            if (_book != null)
            {
                _book.Title = book.Title;
                _book.Description = book.Description;
                _book.IsRead = book.IsRead;
                _book.DateRead = book.IsRead ? book.DateRead.Value : null;
                _book.Rate = book.IsRead ? book.Rate.Value : null;
                _book.Genre = book.Genre;
                _book.Author = book.Author;
                _book.CoverUrl = book.CoverUrl;
                _context.SaveChanges();
            }
            return _book;
        } 
        #endregion
        public void DeleteById(int bookId)
        {
            var _book = _context.Books.FirstOrDefault(n => n.Id == bookId);
            if( _book != null)
            {
                _context.Books.Remove(_book);
                _context.SaveChanges() ;    
            }
        }
    }
}
