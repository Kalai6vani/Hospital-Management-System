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
    public class MedicineController : ControllerBase
    {
        private readonly HospitalDbContext _context;

        public MedicineController(HospitalDbContext context)
        {
            _context = context;
        }

        //add medicine to hospital - admin
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> AddMedicine(int hospitalId, [FromBody] MedicineDTO medicineDto)
        {
            var hospital = await _context.Hospitals.FindAsync(hospitalId);
            if (hospital == null)
            {
                return NotFound("Hospital not found.");
            }

            var medicine = new Medicine
            {
                Name = medicineDto.Name,
                Stock = medicineDto.Stock,
                Price = medicineDto.Price,
                HospitalId = hospitalId
            };

            _context.Medicines.Add(medicine);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                medicine.Id,
                medicine.Name,
                medicine.Stock,
                medicine.HospitalId
            });
        }

        //get medicine by id - admin
        [Authorize(Roles = "Admin, Patient")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetMedicine(int id)
        {
            var medicine = await _context.Medicines.FindAsync(id);
            if (medicine == null)
            {
                return NotFound("Medicine not found.");
            }

            return Ok(medicine);
        }

        //get all medicines - admin/patient
        [Authorize(Roles = "Admin, Patient")]
        [HttpGet]
        public async Task<IActionResult> GetAllMedicines()
        {
            var medicines = await _context.Medicines.ToListAsync(); //list 
            return Ok(medicines);
        }

        //update medicine by id - admin
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateMedicine(int id, [FromBody] MedicineDTO medicineDto)
        {
            var medicine = await _context.Medicines.FindAsync(id);
            if (medicine == null)
            {
                return NotFound("Medicine not found.");
            }

            medicine.Name = medicineDto.Name;
            medicine.Stock = medicineDto.Stock;
            medicine.Price= medicineDto.Price;

            _context.Medicines.Update(medicine);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                medicine.Id,
                medicine.Name,
                medicine.Stock,
                medicine.HospitalId
            });
        }

        //delete medicine by id - admin
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMedicine(int id)
        {
            var medicine = await _context.Medicines.FindAsync(id);
            if (medicine == null)
            {
                return NotFound("Medicine not found.");
            }

            _context.Medicines.Remove(medicine);
            await _context.SaveChangesAsync();

            return Ok("Medicine deleted successfully.");
        }

        //get medicines by hospital - admin/patient
        [Authorize(Roles = "Admin, Patient")]
        [HttpGet("hospital/{hospitalId}")]
        public async Task<IActionResult> GetMedicinesByHospital(int hospitalId)
        {
            var medicines = await _context.Medicines
                .Where(m => m.HospitalId == hospitalId)
                .ToListAsync();

            if (medicines == null || medicines.Count == 0)
            {
                return NotFound("No medicines found for this hospital.");
            }

            return Ok(medicines);
        }
        /*
        [HttpPost("purchase")]
        public async Task<IActionResult> PurchaseMedicine(BillingDTO model)
        {
            var patient = await _context.Patients.FindAsync(model.PatientId);
            if (patient == null) return NotFound("Patient not found.");

            decimal totalAmount = 0;
            foreach (var item in model.BillingDetails)
            {
                var medicine = await _context.Medicines.FindAsync(item.MedicineId);
                if (medicine == null || medicine.Stock < item.Quantity)
                    return BadRequest($"Medicine {item.MedicineId} is not available.");

                medicine.Stock -= item.Quantity;
                totalAmount += item.Quantity * medicine.Price;
            }

            var billing = new Billing
            {
                PatientId = model.PatientId,
                TotalAmount = totalAmount,
                BillingDetails = model.BillingDetails.Select(bd => new BillingDetail
                {
                    MedicineId = bd.MedicineId,
                    Quantity = bd.Quantity,
                    Price = _context.Medicines.Find(bd.MedicineId).Price
                }).ToList()
            };

            _context.Billings.Add(billing);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Purchase successful", TotalAmount = totalAmount });
        }*/

        //purchase medicine - patient
        [Authorize(Roles = "Patient")]
        [HttpPost("purchase")]
        public async Task<IActionResult> PurchaseMedicine(int patientId, int medicineId, int quantity)
        {
            // Validate patient
            var patient = await _context.Patients.FindAsync(patientId);
            if (patient == null) return NotFound("Patient not found.");

            // Validate medicine
            var medicine = await _context.Medicines.FindAsync(medicineId);
            if (medicine == null) return NotFound("Medicine not found.");
            if (medicine.Stock < quantity) return BadRequest("Not enough stock available.");

            // Update medicine stock
            medicine.Stock -= quantity;

            // Calculate total amount
            decimal totalAmount = quantity * medicine.Price;

            // Create a billing record
            var billing = new Billing
            {
                PatientId = patientId,
                TotalAmount = totalAmount,
                BillingDetails = new List<BillingDetail>
        {
            new BillingDetail
            {
                MedicineId = medicineId,
                Quantity = quantity,
                Price = medicine.Price
            }
        }
            };

            // Save changes to database
            _context.Billings.Add(billing);
            await _context.SaveChangesAsync();

            // Return billing details in response
            return Ok(new
            {
                Message = "Purchase successful",
                PatientId = patientId,
                MedicineId = medicineId,
                Quantity = quantity,
                TotalAmount = totalAmount,
                RemainingStock = medicine.Stock
            });
        }
    }
}

