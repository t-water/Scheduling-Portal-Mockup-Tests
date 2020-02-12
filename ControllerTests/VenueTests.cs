using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TEServerTest.Controllers;
using TEServerTest.Data;
using TEServerTest.Models;
using Xunit;

namespace Testing
{
    public class VenueTests
    {
        [Fact]
        public async Task Index_Get_ReturnsViewResult_WithAListOfVenues()
        {
            var mockRepo = new Mock<IVenueRepository>();
            mockRepo.Setup(repo => repo.GetVenuesAsync())
                .ReturnsAsync(GetTestVenues());
            var controller = new VenueController(mockRepo.Object);

            var result = await controller.Index();

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<Venue>>(viewResult.ViewData.Model);
            Assert.Equal(2, model.Count());
        }

        [Fact]
        public void Create_Get_ReturnsViewResultWithNoModel()
        {
            var controller = new VenueController(null);

            var result = controller.Create();

            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Null(viewResult.ViewData.Model);
        }

        [Fact]
        public async Task Create_Post_Unsuccessful_ReturnsViewResult_WithVenue()
        {
            var controller = new VenueController(null);
            controller.ModelState.AddModelError("Name", "Required");
            var mockVenue = new Venue();

            var result = await controller.Create(mockVenue);

            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.IsAssignableFrom<Venue>(viewResult.ViewData.Model);
        }

        [Fact]
        public async Task Create_Post_Successful_ReturnsRedirectToAction()
        {
            var mockRepo = new Mock<IVenueRepository>();
            mockRepo.Setup(repo => repo.Create(GetTestVenues()[0]))
                .ReturnsAsync(GetTestVenues()[0]);
            var controller = new VenueController(mockRepo.Object);
            var mockVenue = GetTestVenues()[0];

            var result = await controller.Create(mockVenue);
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Null(redirectToActionResult.ControllerName);
            Assert.Equal("index", redirectToActionResult.ActionName);
        }

        [Fact]
        public async Task Edit_Get_IDIsNull_ReturnsNotFound()
        {
            var controller = new VenueController(null);
            var result = await controller.Edit(null);
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Edit_Get_VenueIsNull_ReturnsNotFound()
        {
            var mockRepo = new Mock<IVenueRepository>();
            mockRepo.Setup(repo => repo.GetVenueAsync(It.IsAny<int>()))
                .ReturnsAsync(GetTestVenues().Where(v => v.ID == -1).FirstOrDefault());
            var controller = new VenueController(mockRepo.Object);
            var result = await controller.Edit(It.IsAny<int>());
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Edit_Get_VenueFound_ReturnsViewResult()
        {
            var mockRepo = new Mock<IVenueRepository>();
            mockRepo.Setup(repo => repo.GetVenueAsync(It.IsAny<int>()))
                .ReturnsAsync(GetTestVenues()[0]);
            var controller = new VenueController(mockRepo.Object);
            var result = await controller.Edit(It.IsAny<int>());
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.IsAssignableFrom<Venue>(viewResult.ViewData.Model);
        }

        [Fact]
        public async Task Edit_Post_IdsNotEqual_ReturnsNotFound()
        {
            var testVenue = GetTestVenues()[0];
            var controller = new VenueController(null);
            var result = await controller.Edit(testVenue.ID + 1, testVenue);
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Edit_Post_InvalidModelState_ReturnsViewWithModel()
        {
            var controller = new VenueController(null);
            controller.ModelState.AddModelError("Name", "Required");
            var mockVenue = new Venue();

            var result = await controller.Edit(0, mockVenue);

            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.IsAssignableFrom<Venue>(viewResult.ViewData.Model);
        }

        [Fact]
        public async Task Edit_Post_ValidModelState_ReturnsRedirectToActionResult()
        {
            var testVenue = GetTestVenues()[0];
            var mockRepo = new Mock<IVenueRepository>();
            mockRepo.Setup(repo => repo.Update(testVenue))
                .ReturnsAsync(testVenue);
            var controller = new VenueController(mockRepo.Object);
            var result = await controller.Edit(testVenue.ID, testVenue);
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Null(redirectToActionResult.ControllerName);
            Assert.Equal("details", redirectToActionResult.ActionName);
        }

        [Fact]
        public async Task Details_Get_IdNull_ReturnsNotFound()
        {
            var controller = new VenueController(null);
            var result = await controller.Details(null);
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Details_Get_NullVenue_ReturnsNotFound()
        {
            var mockRepo = new Mock<IVenueRepository>();
            mockRepo.Setup(repo => repo.GetVenueAsync(It.IsAny<int>()))
                .ReturnsAsync(GetTestVenues().Where(v => v.ID == -1).FirstOrDefault());
            var controller = new VenueController(mockRepo.Object);
            var result = await controller.Details(It.IsAny<int>());
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Details_Get_Venue_ReturnsViewWithVenue()
        {
            var mockRepo = new Mock<IVenueRepository>();
            mockRepo.Setup(repo => repo.GetVenueAsync(It.IsAny<int>()))
                .ReturnsAsync(GetTestVenues()[0]);
            var controller = new VenueController(mockRepo.Object);
            var result = await controller.Details(It.IsAny<int>());
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.IsAssignableFrom<Venue>(viewResult.ViewData.Model);
        }

        [Fact]
        public async Task Delete_Get_IdNull_ReturnsNotFound()
        {
            var controller = new VenueController(null);
            var result = await controller.Delete(null);
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Delete_Get_NullVenue_ReturnsNotFound()
        {
            var mockRepo = new Mock<IVenueRepository>();
            mockRepo.Setup(repo => repo.GetVenueAsync(It.IsAny<int>()))
                .ReturnsAsync(GetTestVenues().Where(v => v.ID == -1).FirstOrDefault());
            var controller = new VenueController(mockRepo.Object);
            var result = await controller.Delete(It.IsAny<int>());
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Delete_Get_Venue_ReturnsViewWithVenue()
        {
            var mockRepo = new Mock<IVenueRepository>();
            mockRepo.Setup(repo => repo.GetVenueAsync(It.IsAny<int>()))
                .ReturnsAsync(GetTestVenues()[0]);
            var controller = new VenueController(mockRepo.Object);
            var result = await controller.Delete(It.IsAny<int>());
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.IsAssignableFrom<Venue>(viewResult.ViewData.Model);
        }

        [Fact]
        public async Task DeleteConfirmed_ReturnsRedirectToAction()
        {
            var mockRepo = new Mock<IVenueRepository>();
            mockRepo.Setup(repo => repo.GetVenueAsync(It.IsAny<int>()))
                .ReturnsAsync(GetTestVenues()[0]);
            var controller = new VenueController(mockRepo.Object);
            var result = await controller.DeleteConfirmed(It.IsAny<int>());
            var redirectToResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Null(redirectToResult.ControllerName);
            Assert.Equal("index", redirectToResult.ActionName);
        }

        private List<Venue> GetTestVenues()
        {
            var venues = new List<Venue>();
            venues.Add(new Venue()
            {
                ID = 1,
                Name = "Test Venue 1",
                Abbreviation = "T1"
            });
            venues.Add(new Venue()
            {
                ID = 2,
                Name = "Test Venue 2",
                Abbreviation = "T2"
            });
            return venues;
        }
    }
}
