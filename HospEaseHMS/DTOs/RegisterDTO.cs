﻿namespace HospEaseHMS.DTOs
{
    public class RegisterDTO
    {
        public string Name {  get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Role {  get; set; } //Patient or Doctor
    }
}
