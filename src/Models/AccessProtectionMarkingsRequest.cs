using StockportGovUK.NetStandard.Models.Addresses;
using System.ComponentModel.DataAnnotations;

namespace access_protection_service.Models
{
    public class AccessProtectionMarkingsRequest
    {
        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        public string Phone { get; set; }

        [Required]
        public string FurtherLocationDetails { get; set; }

        [Required]
        public Address StreetAddress { get; set; }

        [Required]
        public Address CustomersAddress { get; set; }
    }
}
