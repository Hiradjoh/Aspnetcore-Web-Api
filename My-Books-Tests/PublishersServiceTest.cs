using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using My_books;
using My_books.Data.Models;
using My_books.Data.Services;
using My_books.Data.ViewModels;
using My_books.Exceptions;

namespace My_Books_Tests
{
    public class PublishersServiceTest
    {
        private static DbContextOptions<ProjectDbContext> dbContextOptions = new DbContextOptionsBuilder<ProjectDbContext>()
            .UseInMemoryDatabase(databaseName: "BookDbTest")
            .Options;//create the DB context options and then we pass the options as aparameter to the DB context.

        ProjectDbContext context;
        PublisherService publishersService;

        [OneTimeSetUp]
        public void Setup()
        {
            context = new ProjectDbContext(dbContextOptions);
            context.Database.EnsureCreated();

            SeedDatabase();// add data to the database 

            publishersService = new PublisherService(context);

        }
        #region [-GetAllPublishersTest-]
        [Test, Order(1)]
        public void GetAllpublishers_WithNoSortBy_WithNoSeachString_WithNoPageNumber_Test()
        {
            var result = publishersService.GetAllPublishers("", "", null);
            // chera gozashtim 5 chon ba inke 6 ta publisher darim to seed  vali dakhel service goftim page size = 5 bashe va 5 ta barmigardoone 
            Assert.That(result.Count, Is.EqualTo(5));


        }

        [Test, Order(2)]
        public void GetAllpublishers_WithNoSortBy_WithNoSeachString_WithPageNumber_Test()
        {
            var result = publishersService.GetAllPublishers("", "", 2);
            Assert.That(result.Count, Is.EqualTo(1));


        }
        [Test, Order(3)]
        public void GetAllpublishers_WithNoSortBy_WithNoSortBy_WithSeachString_WithNoPageNumber_Test()
        {
            var result = publishersService.GetAllPublishers("", "3", null);
            Assert.That(result.Count, Is.EqualTo(1)); //inja chon khodemon seed dadim midonim 1 doone publisher3 voojood dare sar hamin result goftim barabar ba 3 bashe 
            Assert.That(result.FirstOrDefault().Name, Is.EqualTo("Publisher 3")); //inja migim esm publisher barabar ba Publisher 3 bashe 

        }

        [Test, Order(4)]
        public void GetAllpublishers_WithSortBy_WithNoSeachString_WithNoPageNumber_Test()
        {
            var result = publishersService.GetAllPublishers("name_desc", "", null);
            Assert.That(result.Count, Is.EqualTo(5));
            Assert.That(result.FirstOrDefault().Name, Is.EqualTo("Publisher 6"));

        }
        #endregion
        [Test, Order(5)]
        public void GetPublisher_WithoutResponse_Test()
        {

            var result = publishersService.GetPublisherById(99);
            Assert.That(result, Is.Null);
        }
        [Test, Order(6)]
        public void GetPublisher_WithResponse_Test()
        {
            var result = publishersService.GetPublisherById(1);

            Assert.That(result.Id, Is.EqualTo(1));
            Assert.That(result.Name, Is.EqualTo("Publisher 1"));
        }

        [Test, Order(7)]
        public void AddPublisher_WithException_Test()
        {
            var newPublisher = new PublisherVM()
            {
                Name = "123 With Exception",
                AddedByUserId = userId
            };

            Assert.That(() => publishersService.AddPublisher(newPublisher)
            , Throws.Exception.TypeOf<PublisherNameException>().With.Message.EqualTo("Name Starts With Number"));
        }

        [Test, Order(8)]
        public void AddPublisher_WithoutException_Test()
        {
            var newPublisher = new PublisherVM()
            {
                Name = "WithoutException"
            };
            var result = publishersService.AddPublisher(newPublisher);
            Assert.That(result.Name, Does.StartWith("Without"));

        }

        [Test, Order(9)]
        public void GetPublisherData_Test()
        {
            var result = publishersService.GetPublisherData(1);
            Assert.That(result.Name, Is.EqualTo("Publisher 1"));
            Assert.That(result.BookAuthors, Is.Not.Empty);
            Assert.That(result.BookAuthors.Count, Is.GreaterThan(0));
            Assert.That(result.BookAuthors.OrderBy(n => n.BookName).FirstOrDefault().BookName, Is.EqualTo("Book 1 Title"));
        }

        [Test, Order(10)]
        public void DeletePublisher_WithException_Test()
        {

            int id = 30;

            Assert.That(() => publishersService.DeletePublisherById(id),
                        Throws.Exception.Message.EqualTo("The Publisher with id 30 does not exist"));
        }

        [Test, Order(11)]
        public void DeletePublisherWithoutException_Test()
        {
            int id = 1; 
            publishersService.DeletePublisherById(id);
            var publisher = publishersService.GetPublisherById(id);
            Assert.That(publisher, Is.Null);
        }




        [OneTimeTearDown]
        public void Cleanup() //cleanup data base after test 
        {
            if (context != null)
            {
                context.Database.EnsureDeleted();
                context.Dispose();
            }
        }

        //[Test]
        //public void Test1()
        //{
        //    Assert.Pass();
        //}

        private void SeedDatabase()
        {
            var publishers = new List<Publisher>
            {
                    new Publisher() {
                        Id = 1,
                        Name = "Publisher 1"
                    },
                    new Publisher() {
                        Id = 2,
                        Name = "Publisher 2"
                    },
                    new Publisher() {
                        Id = 3,
                        Name = "Publisher 3"
                    },
                    new Publisher() {
                        Id = 4,
                        Name = "Publisher 4"
                    },
                    new Publisher() {
                        Id = 5,
                        Name = "Publisher 5"
                    },
                    new Publisher() {
                        Id = 6,
                        Name = "Publisher 6"
                    },
            };
            context.Publishers.AddRange(publishers);

            var authors = new List<Author>()
            {
                new Author()
                {
                    Id = 1,
                    FullName = "Author 1"
                },
                new Author()
                {
                    Id = 2,
                    FullName = "Author 2"
                }
            };
            context.Authors.AddRange(authors);


            var books = new List<Book>()
            {
                new Book()
                {
                    Id = 1,
                    Title = "Book 1 Title",
                    Description = "Book 1 Description",
                    IsRead = false,
                    Genre = "Genre",
                    CoverUrl = "https://...",
                    DateAdded = DateTime.Now.AddDays(-10),
                    PublisherId = 1
                },
                new Book()
                {
                    Id = 2,
                    Title = "Book 2 Title",
                    Description = "Book 2 Description",
                    IsRead = false,
                    Genre = "Genre",
                    CoverUrl = "https://...",
                    DateAdded = DateTime.Now.AddDays(-10),
                    PublisherId = 1
                }
            };
            context.Books.AddRange(books);

            var books_authors = new List<Book_Author>()
            {
                new Book_Author()
                {
                    Id = 1,
                    BookId = 1,
                    AuthorId = 1
                },
                new Book_Author()
                {
                    Id = 2,
                    BookId = 1,
                    AuthorId = 2
                },
                new Book_Author()
                {
                    Id = 3,
                    BookId = 2,
                    AuthorId = 2
                },
            };
            context.Book_Authors.AddRange(books_authors);


            context.SaveChanges();
        }

    }
}


