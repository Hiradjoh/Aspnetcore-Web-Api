using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using My_books;
using My_books.Controllers;
using My_books.Data.Models;
using My_books.Data.Services;
using My_books.Data.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace My_Books_Tests
{
    public class PublishersControllerTest
    {
        private static DbContextOptions<ProjectDbContext> dbContextOptions = new DbContextOptionsBuilder<ProjectDbContext>()
            .UseInMemoryDatabase(databaseName: "BookDbControllerTest")
            .Options;//create the DB context options and then we pass the options as aparameter to the DB context.
        PublisherService publishersService;
        ProjectDbContext context;
        PublisherController publisherController;
        [OneTimeSetUp]
        public void Setup()
        {
            context = new ProjectDbContext(dbContextOptions);
            context.Database.EnsureCreated();

            SeedDatabase();// add data to the database 

            publishersService = new PublisherService(context);
            publisherController = new PublisherController(publishersService, new NullLogger<PublisherController>());

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



        [Test, Order(1)]
        public void HTTPGET_GetAllpublishers_WithSortBy_WithSeachString_WithPageNumber_ReturnOK_Test()
        {
            IActionResult actionResult = publisherController.GetAllublishers("name_desc", "Publisher", 1);

            Assert.That(actionResult, Is.TypeOf<OkObjectResult>());
            var actionResultData = (actionResult as OkObjectResult).Value as List<Publisher>;
            Assert.That(actionResultData.First().Name, Is.EqualTo("Publisher 6"));
            Assert.That(actionResultData.First().Id, Is.EqualTo(6));
            Assert.That(actionResultData.Count, Is.EqualTo(5));


            IActionResult actionResultSeccondPage = publisherController.GetAllublishers("name_desc", "Publisher", 2);

            Assert.That(actionResultSeccondPage, Is.TypeOf<OkObjectResult>());
            var actionResultDataSeccondPage = (actionResultSeccondPage as OkObjectResult).Value as List<Publisher>;
            Assert.That(actionResultDataSeccondPage.First().Name, Is.EqualTo("Publisher 1"));
            Assert.That(actionResultDataSeccondPage.First().Id, Is.EqualTo(1));
            Assert.That(actionResultDataSeccondPage.Count, Is.EqualTo(1));
        }
        [Test, Order(2)]
        public void HTTPGET_GetPublisherById_ReturnOK_Test()
        {
            IActionResult actionResult = publisherController.GetPublisherById(3);
            Assert.That(actionResult, Is.TypeOf<OkObjectResult>());
            var actionResultData = (actionResult as OkObjectResult).Value as Publisher;

            Assert.That(actionResultData.Id, Is.EqualTo(3));
            Assert.That(actionResultData.Name, Is.EqualTo("Publisher 3"));
        }

        [Test, Order(3)]
        public void HTTPGET_GetPublisherById_ReturnNotFound_Test()
        {
            IActionResult actionResult = publisherController.GetPublisherById(10);
            Assert.That(actionResult, Is.TypeOf<NotFoundResult>());

        }
        [Test, Order(4)]
        public void HTTPPOST_AddPublisher_ReturnCreated_Test()
        {
            var newPublisherVM = new PublisherVM()
            {
                Name = "New Publisher"
            };
            IActionResult actionResult = publisherController.AddPublisher(newPublisherVM);
            Assert.That(actionResult, Is.TypeOf<CreatedResult>());
            var actionResultData = (actionResult as CreatedResult).Value as Publisher;
            Assert.That(actionResultData.Id, Is.EqualTo(7));
            Assert.That(actionResultData.Name, Is.EqualTo("New Publisher"));
        }
        [Test, Order(5)]
        public void HTTPPOST_AddPublisher_ReturnBadRequest_Test()
        {
            var newPublisherVM = new PublisherVM()
            {
                Name = "123 Publisher"
            };
            IActionResult actionResult = publisherController.AddPublisher(newPublisherVM);
            Assert.That(actionResult, Is.TypeOf<BadRequestObjectResult>());
        }
        [Test, Order(6)]
        public void HTTPDELETE_DeletePublisherById_ReturnOk_Test()
        {
         
            IActionResult actionResult = publisherController.DeletePublisherById(1);
            Assert.That(actionResult, Is.TypeOf<OkResult>());

        }
        [Test, Order(7)]
        public void HTTPDELETE_DeletePublisherById_ReturnBadRequest_Test()
        {

            IActionResult actionResult = publisherController.DeletePublisherById(12);
            Assert.That(actionResult, Is.TypeOf<BadRequestObjectResult>());

        }




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



            context.SaveChanges();
        }
    }

}
