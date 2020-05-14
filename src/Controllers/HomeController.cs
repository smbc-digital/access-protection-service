using access_protection_service.Models;
using access_protection_service.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using StockportGovUK.AspNetCore.Attributes.TokenAuthentication;
using System;
using System.Threading.Tasks;

namespace access_protection_service.Controllers
{
    [Produces("application/json")]
    [Route("api/v1/[Controller]")]
    [ApiController]
    [TokenAuthentication]
    public class HomeController : ControllerBase
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IAccessProtectionService _accessProtectionService;
    
        public HomeController(ILogger<HomeController> logger, IAccessProtectionService accessProtectionService)
        {
            _logger = logger;
            _accessProtectionService = accessProtectionService;
        }
        
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] AccessProtectionMarkingsRequest accessProtectionMarkingsRequest)
        {
            _logger.LogDebug(JsonConvert.SerializeObject(accessProtectionMarkingsRequest));

            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            try
            {
                var result = await _accessProtectionService.CreateCase(accessProtectionMarkingsRequest);
                _logger.LogWarning($"Case result: { result }");
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogWarning($"HomeContoller:Post, Case an exception has occurred while calling CreateCase, ex: {ex}");
                return StatusCode(500);
            }
        }
    }
}