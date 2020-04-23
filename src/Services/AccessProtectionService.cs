
using access_protection_service.Models;
using StockportGovUK.NetStandard.Gateways.VerintServiceGateway;
using StockportGovUK.NetStandard.Models.Verint;
using System;
using System.Threading.Tasks;

namespace access_protection_service.Services
{
    public class AccessProtectionService : IAccessProtectionService
    {
        private readonly IVerintServiceGateway _VerintServiceGateway;

        public AccessProtectionService(IVerintServiceGateway verintServiceGateway)
        {
            _VerintServiceGateway = verintServiceGateway;
        }
        public async Task<string> CreateCase(AccessProtectionMarkingsRequest accessProtectionMarkingsRequest)
        {
            var description = $@"FirstName: {accessProtectionMarkingsRequest.FirstName}
                                LastName: { accessProtectionMarkingsRequest.LastName}
                                Email: {accessProtectionMarkingsRequest.Email}
                                Phone: {accessProtectionMarkingsRequest.Phone}
                                FurtherLlocationDetails {accessProtectionMarkingsRequest.FurtherLocationDetails}";

            if (accessProtectionMarkingsRequest.CustomersAddress != null)
            {
                description += $@"AddressLine1: {accessProtectionMarkingsRequest.CustomersAddress.AddressLine1}
                                AddressLine2: {accessProtectionMarkingsRequest.CustomersAddress.AddressLine2}
                                Town: {accessProtectionMarkingsRequest.CustomersAddress.Town}
                                Postcode; {accessProtectionMarkingsRequest.CustomersAddress.Postcode}
                                SelectedAddress: {accessProtectionMarkingsRequest.CustomersAddress.SelectedAddress}";
            }

            var crmCase = new Case
            {
                EventCode = 4000031,
                EventTitle = "Basic Verint Case",
                Description = description,
                Street = new Street
                {
                    Reference = accessProtectionMarkingsRequest.StreetAddress.PlaceRef
                }
            };

            if (!string.IsNullOrEmpty(accessProtectionMarkingsRequest.FirstName) && !string.IsNullOrEmpty(accessProtectionMarkingsRequest.LastName))
            {
                crmCase.Customer = new Customer
                {
                    Forename = accessProtectionMarkingsRequest.FirstName,
                    Surname = accessProtectionMarkingsRequest.LastName
                };

                if (!string.IsNullOrEmpty(accessProtectionMarkingsRequest.Email))
                {
                    crmCase.Customer.Email = accessProtectionMarkingsRequest.Email;
                }

                if (!string.IsNullOrEmpty(accessProtectionMarkingsRequest.Phone))
                {
                    crmCase.Customer.Telephone = accessProtectionMarkingsRequest.Phone;
                }

                if (string.IsNullOrEmpty(accessProtectionMarkingsRequest.CustomersAddress.PlaceRef))
                {
                    crmCase.Customer.Address = new Address
                    {
                        AddressLine1 = accessProtectionMarkingsRequest.CustomersAddress.AddressLine1,
                        AddressLine2 = accessProtectionMarkingsRequest.CustomersAddress.AddressLine2,
                        AddressLine3 = accessProtectionMarkingsRequest.CustomersAddress.Town,
                        Postcode = accessProtectionMarkingsRequest.CustomersAddress.Postcode,
                    };
                }
                else
                {
                    crmCase.Customer.Address = new Address
                    {
                        Reference = accessProtectionMarkingsRequest.CustomersAddress.PlaceRef,
                        UPRN = accessProtectionMarkingsRequest.CustomersAddress.PlaceRef
                    };
                }
            }

            try
            {
                var response = await _VerintServiceGateway.CreateCase(crmCase);
                //_logger.LogError(JsonConvert.SerializeObject(response));

                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception("Status code not successful");
                }

                return response.ResponseContent;
            }
            catch (Exception ex)
            {
                throw new Exception($"CRMService CreateCase an exception has occured while creating the case in verint service", ex);
            }
        }
    }
}
