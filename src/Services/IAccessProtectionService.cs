
using access_protection_service.Models;
using System.Threading.Tasks;

namespace access_protection_service.Services
{
    public interface IAccessProtectionService
    {
        Task<string> CreateCase(AccessProtectionMarkingsRequest accessProtectionMarkingsRequest);
    }
}
