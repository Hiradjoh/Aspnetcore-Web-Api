using My_books;
using My_books.Data.Models;
using My_books.Data.ViewModels;

namespace My_books.Data.Services
{
    public class AuthorService
    {
        public ProjectDbContext _context;

        #region [-Ctor-]
        public AuthorService(ProjectDbContext context)
        {
            _context = context;
        }
        #endregion

        #region [-Get-All-Authors-]
        public List<Author> GetAllAuthors()
        {
            return _context.Authors.ToList();
        }
        #endregion

        #region [-Get-Author-By-Id-]
        public Author GetAuthorById(int authorId)
        {
            return _context.Authors.FirstOrDefault(n => n.Id == authorId);
        }
        #endregion

        #region [-Post-Author-]
        public void AddAuthor(AuthorVM author)
        {
            var _author = new Author()
            {
                FullName = author.FullName
            };
            _context.Authors.Add(_author);
            _context.SaveChanges();
        }
        #endregion

        #region [-Update-Author-By-Id-]
        public Author UpdateAuthorById(int authorId, AuthorVM author)
        {
            var _author = _context.Authors.FirstOrDefault(n => n.Id == authorId);
            if (_author != null)
            {
                _author.FullName = author.FullName;
                _context.SaveChanges();
            }
            return _author;
        }
        #endregion

        #region [-Delete-Author-By-Id-]
        public void DeleteById(int authorId)
        {
            var _author = _context.Authors.FirstOrDefault(n => n.Id == authorId);
            if (_author != null)
            {
                _context.Authors.Remove(_author);
                _context.SaveChanges();
            }
        }
        #endregion

        #region [-Get-Author-With-Books-By-Id-]
        public AuthorwithBooksVM GetAuthorWithBooks(int authorId)
        {
            var _author = _context.Authors
                .Where(n => n.Id == authorId)
                .Select(n => new AuthorwithBooksVM()
                {
                    FullName = n.FullName,
                    BookTitles = n.Book_Authors.Select(n => n.Book.Title).ToList()
                }).FirstOrDefault();
            return _author;
        }
        #endregion
    }
}
