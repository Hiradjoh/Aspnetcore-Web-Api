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

        public void DeletePublisherById(int publisherId)
        {
            var _publisher = _context.Publishers.FirstOrDefault(n => n.Id == publisherId);
            if (_publisher != null)
            {
                _context.Publishers.Remove(_publisher);
                _context.SaveChanges();
            }
        }

    }
}

