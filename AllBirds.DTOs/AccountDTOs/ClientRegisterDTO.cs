using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AllBirds.DTOs.AccountDTOs
{
    public class ClientRegisterDTO
    {
        public int Id { get; set; } = 0;

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required, DataType(DataType.Password), Compare("Password", ErrorMessage = "Passwords do not match.")]
        public string ConfirmPassword { get; set; }

        //[StringLength(15, MinimumLength = 8, ErrorMessage = "Phone Number Must Be Between 8 And 15")]
        //public string? PhoneNumber { get; set; }

        [StringLength(15, MinimumLength = 3)]
        public string? FirstName { get; set; }

        [StringLength(15, MinimumLength = 3)]
        public string? LastName { get; set; }
    }
}
