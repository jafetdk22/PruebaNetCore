using CompanyAPI.Models.Response;
using CompanyAPI.Services.ExcelToSB;
using Microsoft.AspNetCore.Mvc;

namespace CompanyAPI.Controllers
{
    [Route("/api/v1/[controller]")]
    [ApiController]
    public class ChargesController : Controller
    {
        private readonly ICSVToDBService _csvToDBService;

        public ChargesController(ICSVToDBService csvToDBService)
        {
            _csvToDBService = csvToDBService;
        }
        [HttpPost("import")]
        [ProducesResponseType(typeof(GenericResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(GenericResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ImportCsv([FromForm] FileUploadModel model)
        {
            var file = model.File;
            if (file == null || file.Length == 0)
            {
                return BadRequest(new { success = false, message = "No file was provided." });
            }

            var result = await _csvToDBService.ImportCsvAsync(file);

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

    }
}
