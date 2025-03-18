using HospEaseHMS.Controllers;
using HospEaseHMS.Data;
using HospEaseHMS.DTOs;
using HospEaseHMS.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HospEaseHMS.Tests.Controllers
{
    [TestFixture]
    public class HospitalControllerTests
    {
        private HospitalDbContext _context;
        private Mock<ILogger<HospitalController>> _mockLogger;
        private HospitalController _controller;

        [SetUp]
        public void SetUp()
        {
            var options = new DbContextOptionsBuilder<HospitalDbContext>()
                .UseInMemoryDatabase(databaseName: "HospitalTest")
                .Options;

            _context = new HospitalDbContext(options);
            _mockLogger = new Mock<ILogger<HospitalController>>();
            _controller = new HospitalController(_context);

            ClearDatabase();
            SeedDatabase();
        }

        [TearDown]
        public void TearDown()
        {
            _context.Dispose();
        }

        private void ClearDatabase()
        {
            _context.Hospitals.RemoveRange(_context.Hospitals);
            _context.SaveChanges();
        }

        private void SeedDatabase()
        {
            var hospital = new Hospital
            {
                Id = 1,
                Name = "Test Hospital",
                Address = "123 Test St",
                Assets = new List<HospitalAsset>(),
                Medicines = new List<Medicine>()
            };

            _context.Hospitals.Add(hospital);
            _context.SaveChanges();
        }
        [Test]
        public async Task CreateHospital_ReturnsCreatedAtActionResult()
        {
            // Arrange
            var hospitalDto = new HospitalDTO
            {
                Name = "New Hospital",
                Address = "456 New St"
            };

            // Act
            var result = await _controller.CreateHospital(hospitalDto);

            // Assert
            Assert.That(result, Is.InstanceOf<CreatedAtActionResult>());
            var createdResult = result as CreatedAtActionResult;
            Assert.That(createdResult.Value, Is.InstanceOf<Hospital>());
            var hospital = createdResult.Value as Hospital;
            Assert.That(hospital.Name, Is.EqualTo("New Hospital"));
            Assert.That(hospital.Address, Is.EqualTo("456 New St"));
        }

        [Test]
        public async Task GetAllHospitals_ReturnsOkResult_WithListOfHospitals()
        {
            // Act
            var result = await _controller.GetAllHospitals();

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var okResult = result as OkObjectResult;
            Assert.That(okResult.Value, Is.InstanceOf<List<Hospital>>());
            var hospitals = okResult.Value as List<Hospital>;
            Assert.That(hospitals.Count, Is.EqualTo(1));
        }
        /*
        [Test]
        public async Task UpdateHospital_ReturnsOkResult()
        {
            // Arrange
            var updatedHospitalDto = new HospitalDTO
            {
                Name = "Updated Hospital",
                Address = "789 Updated St"
            };

            // Act
            var result = await _controller.UpdateHospital(1, updatedHospitalDto);

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var okResult = result as OkObjectResult;
            Assert.That(okResult.Value, Is.InstanceOf<object>());
            var message = okResult.Value as dynamic;
            Assert.That(message.message, Is.EqualTo("Hospital details updated successfully"));
        }


        [Test]
        public async Task DeleteHospital_ReturnsOkResult()
        {
            // Act
            var result = await _controller.DeleteHospital(1);

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var okResult = result as OkObjectResult;
            Assert.That(okResult.Value, Is.InstanceOf<object>());
            var message = okResult.Value as dynamic;
            Assert.That(message.message, Is.EqualTo("Hospital details deleted successfully"));
        }
        */
    }
}
