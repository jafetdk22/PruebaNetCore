using CompanyAPI.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Text;

namespace CompanyAPI.Services.DBtoCSV
{
    public class DBtoCSVService: IDBtoCSVService
    {
        private readonly CompanyChargesDbContext _dbContext;
        private readonly string connectionString;

        public DBtoCSVService(CompanyChargesDbContext dbContext)
        {
            _dbContext = dbContext;
            connectionString = _dbContext.Database.GetDbConnection().ConnectionString;
        }

        public async Task<string> GetCargoDataAsCsv()
        {
            StringBuilder csvData = new StringBuilder();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();
                using (SqlCommand command = new SqlCommand("GetCargoData", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        if (!reader.HasRows)
                            return string.Empty;

                        // Escribir encabezados
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            csvData.Append(reader.GetName(i) + (i < reader.FieldCount - 1 ? "," : "\n"));
                        }

                        // Escribir filas de datos
                        while (await reader.ReadAsync())
                        {
                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                csvData.Append(reader[i]?.ToString().Replace(",", ";") + (i < reader.FieldCount - 1 ? "," : "\n"));
                            }
                        }
                    }
                }
            }

            return csvData.ToString();
        }
    }
}
