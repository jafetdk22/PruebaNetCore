using CompanyAPI.Models.Response;
using CompanyAPI.Services.Calculations;
using Microsoft.AspNetCore.Mvc;

namespace CompanyAPI.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class CalculationController : Controller
    {
        private readonly ICalculationService _calculationService;

        public CalculationController(ICalculationService calculationService)
        {
            _calculationService = calculationService;
        }

        //Endpoint para obtener las transacciones diarias
        [HttpGet("daily-transactions")]
        [ProducesResponseType(typeof(IEnumerable<TransactionSummary>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetDailyTransactions()
        {
            try
            {
                var transactions = await _calculationService.GetDailyTransactions();

                if (!transactions.Any())
                {
                    return NotFound("No transactions found.");
                }
                return Ok(transactions);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }
    }
}
