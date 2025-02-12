namespace CompanyAPI.Services.DBtoCSV
{
    public interface IDBtoCSVService
    {
        Task<string> GetCargoDataAsCsv();
    }
}
