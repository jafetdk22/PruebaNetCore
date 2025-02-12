using CompanyAPI.Models.Response;

namespace CompanyAPI.Services.Calculations
{
    public interface ICalculationService
    {
        Task<IEnumerable<TransactionSummary>> GetDailyTransactions();
    }
}
