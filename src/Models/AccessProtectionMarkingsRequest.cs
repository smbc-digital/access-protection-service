namespace access_protection_service.Models
{
    public class AccessProtectionMarkingsRequest
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string FurtherLocationDetails { get; set; }
        public StockportGovUK.NetStandard.Models.Addresses.Address StreetAddress { get; set; }
        public StockportGovUK.NetStandard.Models.Addresses.Address CustomersAddress { get; set; }
    }
}
