using CompanyAPI.Models.Response;

namespace CompanyAPI.Services.ExcelToSB
{
    public interface ICSVToDBService
    {
        Task<GenericResponse> BulkInsertFromCsv(IFormFile file);
        Task<GenericResponse> ImportCsvAsync(IFormFile file);
    }
}
