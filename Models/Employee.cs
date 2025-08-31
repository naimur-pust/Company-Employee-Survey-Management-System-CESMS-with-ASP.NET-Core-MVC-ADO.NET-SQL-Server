using System.ComponentModel.DataAnnotations;

namespace SmartHRIS.Models
{
    public class Employee
    {
        public int EmployeeId { get; set; }

        [Required, StringLength(100)]
        public string Name { get; set; } = default!;

        [Required, EmailAddress, StringLength(100)]
        public string Email { get; set; } = default!;

        [Required, StringLength(15)]
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; } = default!;

        [StringLength(50)]
        public string? City { get; set; }

        [StringLength(50)]
        public string? Country { get; set; }

        [Display(Name = "Zip Code"), StringLength(10)]
        public string? ZipCode { get; set; }
    }
}
