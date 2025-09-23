using My_books.Data.Models;
using My_books.Data.ViewModels;
using My_books.Exceptions;
using System.Net;
using System.Text.RegularExpressions;

namespace My_books.Data.Services
{
    public class PublisherService
    {
        private readonly ProjectDbContext _context;

        #region [-Ctor-]
        public PublisherService(ProjectDbContext context)
        {
            _context = context;
        }
        #endregion

        #region [-Get-All-Publishers-]
        public List<Publisher> GetAllPublishers()
        {
            return _context.Publishers.ToList();
        }
        #endregion

        #region [-Get-Publisher-By-Id-]
        public Publisher GetPublisherById(int id)
        {
            return _context.Publishers.FirstOrDefault(n => n.Id == id);
        }
        #endregion

        #region [-Post-Publisher-]
        public Publisher AddPublisher(PublisherVM publisher)
        {
            if (StringStartsWithNumber(publisher.Name))
                throw new PublisherNameException("Name Starts With Number", publisher.Name);

            var _publisher = new Publisher()
            {
                Name = publisher.Name,
            };
            _context.Publishers.Add(_publisher);
            _context.SaveChanges();
            return _publisher;
        }
        #endregion

        #region [-Update-Publisher-By-Id-]
        public Publisher UpdatePublisherById(int publisherId, PublisherVM publisher)
        {
            var _publisher = _context.Publishers.FirstOrDefault(n => n.Id == publisherId);
            if (_publisher != null)
            {
                _publisher.Name = publisher.Name;
                _context.SaveChanges();
            }
            return _publisher;
        }
        #endregion

        #region [-Delete-Publisher-By-Id-]
        public void DeletePublisherById(int id)
        {
            var _publisher = _context.Publishers.FirstOrDefault(n => n.Id == id);
            if (_publisher != null)
            {
                _context.Publishers.Remove(_publisher);
                _context.SaveChanges();
            }
            else
            {
                throw new Exception($"The Publisher with id {id} does not exist");
            }
        }
        #endregion

        #region [-Get-Publisher-Data-By-Id-]
        public PublisherWithBooksAndAuthorsVM GetPublisherData(int publisherId)
        {
            var _publisherData = _context.Publishers
                .Where(n => n.Id == publisherId)
                .Select(n => new PublisherWithBooksAndAuthorsVM()
                {
                    Name = n.Name,
                    BookAuthors = n.Books.Select(n => new BookAuthorVM()
                    {
                        BookName = n.Title,
                        BookAuthors = n.Book_Authors.Select(n => n.Author.FullName).ToList()
                    }).ToList()
                }).FirstOrDefault();
            return _publisherData;
        }
        #endregion

        #region [-Helper-Methods-]
        private bool StringStartsWithNumber(string name) => (Regex.IsMatch(name, @"^\d"));
        #endregion
    }
}
