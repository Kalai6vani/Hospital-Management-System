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
    public class MedicineControllerTests
    {
        private HospitalDbContext _context;
        private Mock<ILogger<MedicineController>> _mockLogger;
        private MedicineController _controller;

        [SetUp]
        public void SetUp()
        {
            var options = new DbContextOptionsBuilder<HospitalDbContext>()
                .UseInMemoryDatabase(databaseName: "HospitalTest")
                .Options;

            _context = new HospitalDbContext(options);
            _mockLogger = new Mock<ILogger<MedicineController>>();
            _controller = new MedicineController(_context);

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
            _context.Medicines.RemoveRange(_context.Medicines);
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

            var medicine = new Medicine
            {
                Id = 1,
                Name = "Test Medicine",
                Stock = 100,
                Price = 50,
                HospitalId = 1
            };

            _context.Hospitals.Add(hospital);
            _context.Medicines.Add(medicine);
            _context.SaveChanges();
        }
        [Test]
        public async Task GetMedicine_ReturnsOkResult_WithMedicine()
        {
            // Act
            var result = await _controller.GetMedicine(1);

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var okResult = result as OkObjectResult;
            Assert.That(okResult.Value, Is.InstanceOf<Medicine>());
            var medicine = okResult.Value as Medicine;
            Assert.That(medicine.Name, Is.EqualTo("Test Medicine"));
            Assert.That(medicine.Stock, Is.EqualTo(100));
            Assert.That(medicine.Price, Is.EqualTo(50));
        }

        [Test]
        public async Task GetAllMedicines_ReturnsOkResult_WithListOfMedicines()
        {
            // Act
            var result = await _controller.GetAllMedicines();

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var okResult = result as OkObjectResult;
            Assert.That(okResult.Value, Is.InstanceOf<List<Medicine>>());
            var medicines = okResult.Value as List<Medicine>;
            Assert.That(medicines.Count, Is.EqualTo(1));
        }
        /*
        [Test]
        public async Task AddMedicine_ReturnsOkResult()
        {
            // Arrange
            var medicineDto = new MedicineDTO
            {
                Name = "New Medicine",
                Stock = 200,
                Price = 75
            };

            // Act
            var result = await _controller.AddMedicine(1, medicineDto);

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var okResult = result as OkObjectResult;
            Assert.That(okResult.Value, Is.InstanceOf<object>());
            var medicine = okResult.Value as dynamic;
            Assert.That(medicine.Name, Is.EqualTo("New Medicine"));
            Assert.That(medicine.Stock, Is.EqualTo(200));
            Assert.That(medicine.Price, Is.EqualTo(75));
        }


        [Test]
        public async Task UpdateMedicine_ReturnsOkResult()
        {
            // Arrange
            var medicineDto = new MedicineDTO
            {
                Name = "Updated Medicine",
                Stock = 150,
                Price = 60
            };

            // Act
            var result = await _controller.UpdateMedicine(1, medicineDto);

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var okResult = result as OkObjectResult;
            Assert.That(okResult.Value, Is.InstanceOf<object>());
            var medicine = okResult.Value as dynamic;
            Assert.That(medicine.Name, Is.EqualTo("Updated Medicine"));
            Assert.That(medicine.Stock, Is.EqualTo(150));
            Assert.That(medicine.Price, Is.EqualTo(60));
        }
        */

        [Test]
        public async Task DeleteMedicine_ReturnsOkResult()
        {
            // Act
            var result = await _controller.DeleteMedicine(1);

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var okResult = result as OkObjectResult;
            Assert.That(okResult.Value, Is.EqualTo("Medicine deleted successfully."));
        }

        [Test]
        public async Task GetMedicinesByHospital_ReturnsOkResult_WithListOfMedicines()
        {
            // Act
            var result = await _controller.GetMedicinesByHospital(1);

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var okResult = result as OkObjectResult;
            Assert.That(okResult.Value, Is.InstanceOf<List<Medicine>>());
            var medicines = okResult.Value as List<Medicine>;
            Assert.That(medicines.Count, Is.EqualTo(1));
        }
    }
}
