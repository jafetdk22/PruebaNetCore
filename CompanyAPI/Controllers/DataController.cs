using Azure;
using CompanyAPI.Models.Response;
using CompanyAPI.Services.DBtoCSV;
using CompanyAPI.Services.ExcelToSB;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Text;

namespace CompanyAPI.Controllers
{
    [Route("/api/v1/[controller]")]
    [ApiController]
    public class DataController : Controller
    {
        private readonly ICSVToDBService _csvToDBService;
        private readonly IDBtoCSVService _dbToCSVService;

        public DataController(ICSVToDBService csvToDBService, IDBtoCSVService dbToCSVService)
        {
            _csvToDBService = csvToDBService;
            _dbToCSVService = dbToCSVService;
        }

        [HttpPost("Import")]
        [ProducesResponseType(typeof(GenericResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(GenericResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Import([FromForm] FileUploadModel model)
        {
            if (model.File == null || model.File.Length == 0)
            {
                return BadRequest(new GenericResponse
                {
                    Success = false,
                    Message = "No file uploaded."
                });
            }

            // Validar tipo de archivo (por ejemplo, CSV)
            if (!model.File.FileName.EndsWith(".csv"))
            {
                return BadRequest(new GenericResponse
                {
                    Success = false,
                    Message = "Invalid file format. Only CSV files are allowed."
                });
            }

            try
            {
                // Llamada al servicio para insertar el archivo CSV
                var result = await _csvToDBService.BulkInsertFromCsv(model.File);

                if (result.Success)
                {
                    return Ok(result);
                }
                else
                {
                    return BadRequest(result);
                }
            }
            catch (Exception ex)
            {
                // Manejo de errores para capturar cualquier excepción
                return StatusCode((int)HttpStatusCode.InternalServerError, new GenericResponse
                {
                    Success = false,
                    Message = $"An error occurred: {ex.Message}"
                });
            }
        }

        [HttpGet("export-csv")]
        [ProducesResponseType(typeof(GenericResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(GenericResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ExportToCsv()
        {
            var csvContent = await _dbToCSVService.GetCargoDataAsCsv();

            if (string.IsNullOrEmpty(csvContent))
            {
                return NotFound("No hay datos disponibles.");
            }

            byte[] fileBytes = Encoding.UTF8.GetBytes(csvContent);
            string fileName = "cargo_data.csv";

            return File(fileBytes, "text/csv", fileName);
        }

    }
}
