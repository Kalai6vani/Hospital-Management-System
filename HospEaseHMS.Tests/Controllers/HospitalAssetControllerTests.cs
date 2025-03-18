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
    public class HospitalAssetControllerTests
    {
        private HospitalDbContext _context;
        private Mock<ILogger<HospitalAssetController>> _mockLogger;
        private HospitalAssetController _controller;

        [SetUp]
        public void SetUp()
        {
            var options = new DbContextOptionsBuilder<HospitalDbContext>()
                .UseInMemoryDatabase(databaseName: "HospitalTest")
                .Options;

            _context = new HospitalDbContext(options);
            _mockLogger = new Mock<ILogger<HospitalAssetController>>();
            _controller = new HospitalAssetController(_context);

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
            _context.HospitalAssets.RemoveRange(_context.HospitalAssets);
            _context.SaveChanges();
        }

        private void SeedDatabase()
        {
            var hospital = new Hospital
            {
                Id = 1,
                Name = "Test Hospital",
                Address = "123 Test St",
                Assets = new List<HospitalAsset>()
            };

            var asset = new HospitalAsset
            {
                Id = 1,
                Name = "Test Asset",
                Quantity = 10,
                HospitalId = 1
            };

            _context.Hospitals.Add(hospital);
            _context.HospitalAssets.Add(asset);
            _context.SaveChanges();
        }
        
        [Test]
        public async Task GetAllAssets_ReturnsOkResult_WithListOfAssets()
        {
            // Act
            var result = await _controller.GetAllAssets();

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var okResult = result as OkObjectResult;
            Assert.That(okResult.Value, Is.InstanceOf<List<HospitalAsset>>());
            var assets = okResult.Value as List<HospitalAsset>;
            Assert.That(assets.Count, Is.EqualTo(1));
        }
        /*
         [Test]
        public async Task AddAsset_ReturnsOkResult()
        {
            // Arrange
            var assetDto = new HospitalAssetDTO
            {
                Name = "New Asset",
                Quantity = 20
            };

            // Act
            var result = await _controller.AddAsset(1, assetDto);

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var okResult = result as OkObjectResult;
            Assert.That(okResult.Value, Is.InstanceOf<object>());
            var asset = okResult.Value as dynamic;
            Assert.That(asset.Name, Is.EqualTo("New Asset"));
            Assert.That(asset.Quantity, Is.EqualTo(20));
        }

        [Test]
        public async Task UpdateAsset_ReturnsOkResult()
        {
            // Arrange
            var assetDto = new HospitalAssetDTO
            {
                Name = "Updated Asset",
                Quantity = 30
            };

            // Act
            var result = await _controller.UpdateAsset(1, assetDto);

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var okResult = result as OkObjectResult;
            Assert.That(okResult.Value, Is.InstanceOf<object>());
            var asset = okResult.Value as dynamic;
            Assert.That(asset.Name, Is.EqualTo("Updated Asset"));
            Assert.That(asset.Quantity, Is.EqualTo(30));
        }
        */
        [Test]
        public async Task DeleteAsset_ReturnsOkResult()
        {
            // Act
            var result = await _controller.DeleteAsset(1);

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var okResult = result as OkObjectResult;
            Assert.That(okResult.Value, Is.EqualTo("Asset deleted successfully."));
        }

        [Test]
        public async Task GetAssetsByHospital_ReturnsOkResult_WithListOfAssets()
        {
            // Act
            var result = await _controller.GetAssetsByHospital(1);

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var okResult = result as OkObjectResult;
            Assert.That(okResult.Value, Is.InstanceOf<List<HospitalAsset>>());
            var assets = okResult.Value as List<HospitalAsset>;
            Assert.That(assets.Count, Is.EqualTo(1));
        }
    }
}
