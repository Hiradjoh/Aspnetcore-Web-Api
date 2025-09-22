using My_books.Data.Models;
using My_books.Data.ViewModels;
using System.Net;

namespace My_books.Data.Services
{
    public class PublisherService
    {
        private readonly ProjectDbContext _context;

        public PublisherService(ProjectDbContext context)
        {
            _context = context;
        }
        public List<Publisher> GetAllPublishers()
        {
            return _context.Publishers.ToList();

        }
        public Publisher GetPublisherById(int publisherId)
        {
            return _context.Publishers.FirstOrDefault(n => n.Id == publisherId);
        }
        public void AddPublisher(PublisherVM publishers)
        {
            var _publisher = new Publisher()
            {
                Name = publishers.Name,

            };
            _context.Publishers.Add(_publisher);
            _context.SaveChanges();
        }
        public Publisher UpdatePublisherById(int publisherId ,PublisherVM publisher)
        {
            var _publisher = _context.Publishers.FirstOrDefault(n => n.Id == publisherId);
            if (_publisher != null)
            {
                _publisher.Name=publisher.Name;
                _context.SaveChanges();
            }
            return _publisher;
        }

        public void DeletePublisherById(int id)
        {
            var _publisher = _context.Publishers.FirstOrDefault(n => n.Id == id);
            if (_publisher != null)
            {
                _context.Publishers.Remove(_publisher);
                _context.SaveChanges();
            }
        }
        public PublisherWithBooksAndAuthorsVM GetPublisherData(int publisherId)
        {
            var _publisherData=_context.Publishers.Where(n=>n.Id==publisherId)
                .Select(n=> new PublisherWithBooksAndAuthorsVM()
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

    }
}

