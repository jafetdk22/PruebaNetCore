using CompanyAPI.Models.Response;
using CompanyAPI.Services.SetOfNumber;
using Microsoft.AspNetCore.Mvc;

namespace CompanyAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MissingNumberController : Controller
    {
        private readonly ISetofNumberService _setOfNumberService;

        public MissingNumberController(ISetofNumberService setOfNumberService)
        {
            _setOfNumberService = setOfNumberService;
        }

        [HttpPost("extract")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Extract([FromForm] int number)
        {
            if (number < 1 || number > 100)
            {
                return BadRequest("The number must be between 1 and 100.");
            }
            try
            {

                var extractedNumber = await _setOfNumberService.Extract(number);
                return Ok(extractedNumber);
            }
            catch (ArgumentOutOfRangeException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("missing")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetMissingNumber()
        {
            var missingNumber = await _setOfNumberService.CalculateMissingNumber();
            return Ok(missingNumber);
        }
    }
}
