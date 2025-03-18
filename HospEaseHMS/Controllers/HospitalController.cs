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
    public class HospitalController : ControllerBase
    {
        private readonly HospitalDbContext _context;

        public HospitalController(HospitalDbContext context)
        {
            _context = context;
        }

        //add hospital - admin
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> CreateHospital([FromBody] HospitalDTO hospitalDto)
        {
            if (hospitalDto == null)
            {
                return BadRequest("Invalid data.");
            }

            var hospital = new Hospital
            {
                Name = hospitalDto.Name,
                Address = hospitalDto.Address,
               // ContactNumber = hospitalDto.ContactNumber
            };

            _context.Hospitals.Add(hospital);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetHospitalById), new { id = hospital.Id }, hospital);
            //return a 201 Created response, which indicates that a new resource has been
            //successfully created. 
        }

        //get all hospitals - admin/patient/doctor
        [Authorize(Roles = "Admin, Patient, Doctor")]
        [HttpGet]
      //  [Authorize(Roles ="Admin")]
        public async Task<IActionResult> GetAllHospitals()
        {
            var hospitals = await _context.Hospitals.ToListAsync();
            return Ok(hospitals);
        }

        //get hospital by id - admin
        [Authorize(Roles = "Admin, Patient")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetHospitalById(int id)
        {
            var hospital = await _context.Hospitals
                .Include(h => h.Assets)
                .Include(h => h.Medicines)
                .FirstOrDefaultAsync(h => h.Id == id);

            if (hospital == null) return NotFound("Hospital not found");

            return Ok(new
            {
                hospital.Id,
                hospital.Name,
                hospital.Address,
                Assets = hospital.Assets.Select(a => new {a.Id, a.Name, a.Quantity}),
                Medicines=hospital.Medicines.Select(m=>new {m.Id, m.Name,m.Stock, m.Price})
            });
        }

        //update hospital - admin
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateHospital(int id, [FromBody] HospitalDTO updatedHospital)
        {
            var hospital = await _context.Hospitals.FindAsync(id);
            if (hospital == null) return NotFound();

            hospital.Name = updatedHospital.Name;
            hospital.Address = updatedHospital.Address;

            await _context.SaveChangesAsync();
            return Ok(new {message="Hospital details updated successfully"});
        }

        //delete hospital - admin
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteHospital(int id)
        {
            var hospital = await _context.Hospitals.FindAsync(id); 
            if (hospital == null) return NotFound();

            _context.Hospitals.Remove(hospital);
            await _context.SaveChangesAsync();
            return Ok(new { message = "Hospital details deleted successfully" });
        }
    }
}
