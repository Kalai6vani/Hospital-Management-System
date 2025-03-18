using HospEaseHMS.Models;
using Microsoft.EntityFrameworkCore;

namespace HospEaseHMS.Data
{
    public class HospitalDbContext : DbContext
    {
        public HospitalDbContext(DbContextOptions<HospitalDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Admin> Admins { get; set; }
        public DbSet<Doctor> Doctors { get; set; }
        public DbSet<Patient> Patients { get; set; }
        public DbSet<Hospital> Hospitals { get; set; }
        public DbSet<HospitalAsset> HospitalAssets { get; set; }
        public DbSet<Medicine> Medicines { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<MedicalRecord> MedicalRecords { get; set; }
        public DbSet<Billing> Billings { get; set; }
        public DbSet<BillingDetail> BillingDetails { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Enforce one Admin rule
            //  modelBuilder.Entity<Admin>()
            //    .HasCheckConstraint("CHK_Admin", "1 = (SELECT COUNT(*) FROM Admins)");

modelBuilder.Entity<HospitalAsset>()
    .HasOne(h => h.Hospital)
    .WithMany(h => h.Assets)
    .HasForeignKey(h => h.HospitalId)
    .OnDelete(DeleteBehavior.Cascade); //cascade=allowed
                                       /*When the principal entity (e.g., Hospital) is deleted,
                                       all related dependent entities (e.g., Medicine) are also 
                                       automatically deleted.*/

            modelBuilder.Entity<Medicine>()
    .HasOne(m => m.Hospital)
    .WithMany(h => h.Medicines)
    .HasForeignKey(m => m.HospitalId)
    .OnDelete(DeleteBehavior.Cascade);

modelBuilder.Entity<Appointment>()
    .HasOne(a => a.Patient)
    .WithMany(p => p.Appointments)
    .HasForeignKey(a => a.PatientId)
    .OnDelete(DeleteBehavior.Restrict); //cascade=prevented
            /* When the principal entity (e.g., Patient) is deleted, the delete operation is 
             * prevented if there are any related dependent entities (e.g., Appointment). This 
             * ensures that the principal entity cannot be deleted if it has related dependents.*/

            modelBuilder.Entity<Appointment>()
    .HasOne(a => a.Doctor)
    .WithMany()
    .HasForeignKey(a => a.DoctorId)
    .OnDelete(DeleteBehavior.Restrict);

modelBuilder.Entity<MedicalRecord>()
    .HasOne(mr => mr.Patient)
    .WithMany(p => p.MedicalRecords)
    .HasForeignKey(mr => mr.PatientId)
    .OnDelete(DeleteBehavior.Restrict);

modelBuilder.Entity<Billing>()
    .HasOne(b => b.Patient)
    .WithMany()
    .HasForeignKey(b => b.PatientId)
    .OnDelete(DeleteBehavior.Restrict);

modelBuilder.Entity<BillingDetail>()
    .HasOne(bd => bd.Billing)
    .WithMany(b => b.BillingDetails)
    .HasForeignKey(bd => bd.BillingId)
    .OnDelete(DeleteBehavior.Cascade);

modelBuilder.Entity<BillingDetail>()
    .HasOne(bd => bd.Medicine)
    .WithMany()
    .HasForeignKey(bd => bd.MedicineId)
    .OnDelete(DeleteBehavior.Restrict);
}
}
}
