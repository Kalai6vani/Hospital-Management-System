using HospEaseHMS.Data;
using HospEaseHMS.DTOs;
using HospEaseHMS.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HospEaseHMS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HospitalAssetController : ControllerBase
    {
        private readonly HospitalDbContext _context;

        public HospitalAssetController(HospitalDbContext context)
        {
            _context = context;
        }

        //add assets to hospital - admin
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> AddAsset(int hospitalId, [FromBody] HospitalAssetDTO assetDto)
        {
            //[FromBody] - specify that a parameter should be bound from the body of the HTTP request. 
            //necessary because, by default, ASP.NET Core binds parameters from the query string, route data, and
            //form data.

            var hospital = await _context.Hospitals.FindAsync(hospitalId);
            if (hospital == null)
            {
                return NotFound("Hospital not found.");
            }

            var asset = new HospitalAsset
            {
                Name = assetDto.Name,
                Quantity = assetDto.Quantity,
                HospitalId = hospitalId
            };

            _context.HospitalAssets.Add(asset);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                asset.Id,
                asset.Name,
                asset.Quantity,
                asset.HospitalId
            });
        }

        //get all assets - admin/patient
        [Authorize(Roles = "Admin, Patient")]
        [HttpGet]
        public async Task<IActionResult> GetAllAssets()
        {
            var assets = await _context.HospitalAssets.ToListAsync();
            return Ok(assets);
        }


        //update asset by id - admin
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAsset(int id, [FromBody] HospitalAssetDTO assetDto)
        {
            var asset = await _context.HospitalAssets.FindAsync(id);
            if (asset == null)
            {
                return NotFound("Asset not found.");
            }

            asset.Name = assetDto.Name;
            asset.Quantity = assetDto.Quantity;

            _context.HospitalAssets.Update(asset);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                asset.Id,
                asset.Name,
                asset.Quantity,
                asset.HospitalId
            });
        }

        //delete asset by id - admin
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsset(int id)
        {
            var asset = await _context.HospitalAssets.FindAsync(id);
            if (asset == null)
            {
                return NotFound("Asset not found.");
            }

            _context.HospitalAssets.Remove(asset);
            await _context.SaveChangesAsync();

            return Ok("Asset deleted successfully.");
        }

        //get assets by hospital - admin/patient
        [Authorize(Roles = "Admin, Patient")]
        [HttpGet("hospital/{hospitalId}")]
        public async Task<IActionResult> GetAssetsByHospital(int hospitalId)
        {
            var assets = await _context.HospitalAssets
                .Where(a => a.HospitalId == hospitalId)
                .ToListAsync();

            if (assets == null || assets.Count == 0)
            {
                return NotFound("No assets found for this hospital.");
            }

            return Ok(assets);
        }
    }

}

