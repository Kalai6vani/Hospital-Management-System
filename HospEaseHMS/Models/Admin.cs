using Microsoft.EntityFrameworkCore;

namespace HospEaseHMS.Models
{
    public class Admin:User
    {
        // Ensures only one Admin exists
        public static bool Exists { get; set; } = false;

        public Admin()
        {
            if (Exists) { }
                //throw new Exception("Only one admin is allowed.");
            Exists = true;
        }
    }
}
