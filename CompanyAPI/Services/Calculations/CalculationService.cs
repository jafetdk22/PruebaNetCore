using CompanyAPI.Models;
using CompanyAPI.Models.Response;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Dapper;

namespace CompanyAPI.Services.Calculations
{
    public class CalculationService: ICalculationService
    {
        private readonly CompanyChargesDbContext _dbContext;
        private readonly string connectionString;

        public CalculationService(CompanyChargesDbContext dbContext)
        {
            _dbContext = dbContext;
            connectionString = _dbContext.Database.GetDbConnection().ConnectionString;
        }

        public async Task<IEnumerable<TransactionSummary>> GetDailyTransactions()
        {
            using var connection = new SqlConnection(connectionString);
            string query = "SELECT * FROM TotalChargesByDay ORDER BY Transaction_date DESC";

            var result = await connection.QueryAsync<dynamic>(query);
            return result.Select(r => new TransactionSummary
            {
                CompanyId = r.company_id,
                CompanyName = r.company_name,
                TransactionDate = r.transaction_date,
                TotalAmount = r.total_amount
            });
        }
    }
}
