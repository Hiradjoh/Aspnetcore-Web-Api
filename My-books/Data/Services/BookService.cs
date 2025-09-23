using My_books.Data.Models;
using My_books.Data.ViewModels;
using System.Threading;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace My_books.Data.Services
{
    public class BookService
    {
        private readonly ProjectDbContext _context;

        #region [-Ctor-]
        public BookService(ProjectDbContext context)
        {
            _context = context;
        }
        #endregion

        #region [-Get-All-Books-]
        public List<Book> GetAllBooks()
        {
            return _context.Books.ToList();
        }
        #endregion

        #region [-Get-Book-By-Id-]
        public BookWithAuthorsVM GetBookById(int bookId)
        {
            var _bookWithAuthors = _context.Books.Select(book => new BookWithAuthorsVM()
            {
                Title = book.Title,
                Description = book.Description,
                IsRead = book.IsRead,
                DateRead = book.IsRead ? book.DateRead.Value : null,
                Rate = book.IsRead ? book.Rate.Value : null,
                Genre = book.Genre,
                CoverUrl = book.CoverUrl,
                PublisherName = book.Publisher.Name,
                AuthorNames = book.Book_Authors.Select(n => n.Author.FullName).ToList()
            }).FirstOrDefault();
            return _bookWithAuthors;
        }
        #endregion

        #region [-Post-Book-With-Authors-]
        public void AddBookWithAuthors(BookVM book)
        {
            var _book = new Book()
            {
                Title = book.Title,
                Description = book.Description,
                IsRead = book.IsRead,
                DateRead = book.IsRead ? book.DateRead.Value : null,
                Rate = book.IsRead ? book.Rate.Value : null,
                Genre = book.Genre,
                CoverUrl = book.CoverUrl,
                DateAdded = DateTime.Now,
                PublisherId = book.publisherId
            };
            _context.Books.Add(_book);
            _context.SaveChanges();

            foreach (var id in book.AuthorId)
            {
                var _book_author = new Book_Author()
                {
                    BookId = _book.Id,
                    AuthorId = id,
                };
                _context.Book_Authors.Add(_book_author);
            }
            _context.SaveChanges();
        }
        #endregion

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
                _book.CoverUrl = book.CoverUrl;
                _context.SaveChanges();
            }
            return _book;
        }
        #endregion

        #region [-Delete-Book-By-Id-]
        public void DeleteById(int bookId)
        {
            var _book = _context.Books.FirstOrDefault(n => n.Id == bookId);
            if (_book != null)
            {
                _context.Books.Remove(_book);
                _context.SaveChanges();
            }
        }
        #endregion
    }
}
