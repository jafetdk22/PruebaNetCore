using Microsoft.Data.SqlClient;
using System.Globalization;
using CompanyAPI.Models;
using Microsoft.EntityFrameworkCore;
using CsvHelper;
using CompanyAPI.Models.Response;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Data;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.Text.RegularExpressions;

namespace CompanyAPI.Services.ExcelToSB
{
    public class CSVToDBService : ICSVToDBService
    {
        private readonly CompanyChargesDbContext _dbContext;
        private readonly string connectionString;

        public CSVToDBService(CompanyChargesDbContext dbContext)
        {
            _dbContext = dbContext;
            connectionString = _dbContext.Database.GetDbConnection().ConnectionString;
        }

        private static DateTime? ParseDateTime(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return null;

            string[] formats = { "yyyy-MM-dd HH:mm:ss", "yyyy-MM-dd", "yyyy-MM-ddTHH:mm:ss", "yyyyMMdd" };
            DateTime parsedDate;

            foreach (var format in formats)
            {
                if (DateTime.TryParseExact(value, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out parsedDate))
                    return parsedDate;
            }

            throw new FormatException($"Unrecognized date format: {value}");
        }


        private static decimal ValidateAmount(decimal amount)
        {
            decimal maxValue = 99999999999999.99m;
            decimal minValue = -99999999999999.99m;

            if (amount > maxValue)
            {
                amount = maxValue;
            }
            else if (amount < minValue)
            {
                amount = minValue;
            }

            return Math.Round(amount, 2);
        }

        public async Task<GenericResponse> BulkInsertFromCsv(IFormFile file)
        {
            var result = new GenericResponse();
            var errors = new List<string>(); // Lista de errores
            int successCount = 0; // Contador de registros exitosos
            var rowsCount = 0;

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    var rows = new List<Tuple<string, string, string, decimal, string, DateTime?, DateTime?>>();
                    var existingIds = new HashSet<string>(); // Evitar IDs duplicados

                    using (var reader = new StreamReader(file.OpenReadStream()))
                    using (var csv = new CsvReader(reader, new CsvHelper.Configuration.CsvConfiguration(CultureInfo.InvariantCulture)))
                    {
                        var records = csv.GetRecords<dynamic>();

                        foreach (var row in records)
                        {
                            try
                            {
                                var line = rowsCount + 1; // Línea actual
                                var chargeId = row.id?.ToString()?.Trim();
                                var nameCompany = row.name?.ToString()?.Trim();
                                var companyId = row.company_id?.ToString()?.Trim();
                                var amountStr = row.amount?.ToString()?.Trim();
                                var status = row.status?.ToString()?.Trim();
                                DateTime? createdAt = ParseDateTime(row.created_at?.ToString());
                                DateTime? updatedAt = ParseDateTime(row.paid_at?.ToString());

                                // 🔹 **Validación de chargeId**
                                if (string.IsNullOrWhiteSpace(chargeId) || !Regex.IsMatch(chargeId, @"^[a-zA-Z0-9]+$"))
                                {
                                    errors.Add($"Invalid chargeId on line {line}: '{chargeId}'. " +
                                        (string.IsNullOrWhiteSpace(chargeId) ? "It is empty or null." : "It must be alphanumeric."));

                                    continue;
                                }

                                // Evitar IDs duplicados
                                if (existingIds.Contains(chargeId))
                                {
                                    errors.Add($"Row with ID {chargeId}: The ID has already been processed.");
                                    continue;
                                }
                                existingIds.Add(chargeId);

                                // 🔹 **Validación de nameCompany**
                                if (string.IsNullOrWhiteSpace(nameCompany) || !Regex.IsMatch(nameCompany, @"^[a-zA-Z0-9 ]+$"))
                                {
                                    errors.Add($"Invalid nameCompany in chargeId {chargeId}: '{nameCompany}'. " + "It must be alphanumeric and not empty.");

                                    continue;
                                }

                                // 🔹 **Validación de companyId**
                                if (string.IsNullOrWhiteSpace(companyId) || companyId == "*******" || !Regex.IsMatch(companyId, @"^[a-zA-Z0-9]+$"))
                                {
                                    if (string.IsNullOrWhiteSpace(companyId))
                                    {
                                        errors.Add($"Invalid companyId in chargeId {chargeId}: It must not be empty.");
                                    }
                                    else if (companyId == "*******")
                                    {
                                        errors.Add($"Invalid companyId in chargeId {chargeId}: It must not be '*******'.");
                                    }
                                    else
                                    {
                                        errors.Add($"Invalid companyId in chargeId {chargeId}: It must not contain special characters.");
                                    }
                                    continue;
                                }

                                // 🔹 **Validación de amount**
                                if (!decimal.TryParse(amountStr, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal amount) || amount <= 0)
                                {
                                    errors.Add($"Invalid amount in chargeId {chargeId}: '{amountStr}'. It must be a valid decimal number greater than 0.");
                                    continue;
                                }

                                // Agregar fila validada
                                rows.Add(new Tuple<string, string, string, decimal, string, DateTime?, DateTime?>(
                                    chargeId, nameCompany, companyId, amount, status, createdAt, updatedAt));
                                rowsCount++;
                            }
                            catch (Exception ex)
                            {
                                errors.Add($"Error processing the row: {ex.Message}");
                            }
                        }
                    }

                    // 🔹 **Inserción en bloque**
                    var insertQuery = @"INSERT INTO Cargo (id, company_name, company_id, amount, status, created_at, updated_at) 
                              VALUES (@id, @company_name, @company_id, @amount, @status, @created_at, @updated_at)";

                    using (var command = new SqlCommand(insertQuery, conn))
                    {
                        foreach (var row in rows)
                        {
                            try
                            {
                                command.Parameters.Clear();
                                command.Parameters.AddWithValue("@id", row.Item1);
                                command.Parameters.AddWithValue("@company_name", (object)row.Item2 ?? DBNull.Value);
                                command.Parameters.AddWithValue("@company_id", row.Item3);
                                command.Parameters.AddWithValue("@amount", row.Item4);
                                command.Parameters.AddWithValue("@status", row.Item5);
                                command.Parameters.AddWithValue("@created_at", (object)row.Item6 ?? DBNull.Value);
                                command.Parameters.AddWithValue("@updated_at", (object)row.Item7 ?? DBNull.Value);

                                await command.ExecuteNonQueryAsync();
                                successCount++;
                            }
                            catch (Exception ex)
                            {
                                errors.Add($"Error inserting the row with ID {row.Item1}: {ex.Message}");
                            }
                        }
                    }

                    result.SuccessCount = successCount;
                    result.Message = "The bulk insertion was completed successfully.";

                    // Si existen errores, incluirlos en la respuesta
                    if (result.SuccessCount == 0)
                    {
                        result.Success = false;
                        result.Message = "Errors were found during the process.";
                        result.Errors = errors.ToArray();
                    }
                    else if (errors.Count > 0)
                    {
                        result.Message += " Errors were found during the process.";
                        result.Errors = errors.ToArray();
                    }
                }
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = $"Error during bulk insertion: {ex.Message}";
                result.Errors = new[] { ex.Message };
            }

            return result;
        }

        public async Task<GenericResponse> ImportCsvAsync(IFormFile file)
        {
            var response = new GenericResponse();

            if (file == null || file.Length == 0)
            {
                response.Success = false;
                response.Message = "No file was provided.";
                return response;
            }

            try
            {
                using (var stream = new StreamReader(file.OpenReadStream(), Encoding.UTF8))
                {
                    string line;
                    bool isFirstLine = true;
                    int successCount = 0;
                    var errors = new List<string>();

                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        await connection.OpenAsync();

                        while ((line = await stream.ReadLineAsync()) != null)
                        {
                            if (isFirstLine)
                            {
                                isFirstLine = false;
                                continue;
                            }

                            var values = line.Split(',');

                            if (values.Length < 6)
                            {
                                continue;
                            }

                            try
                            {
                                string chargeId = values[0].Trim();
                                string nameCompany = values[1].Trim();
                                string companyId = values[2].Trim();
                                decimal amount;
                                string status = values[4].Trim();
                                DateTime createdAt = ParseDateTime(values[5]) ?? throw new FormatException($"Invalid date format in CreatedAt: {values[5]}");
                                DateTime? updatedAt = ParseDateTime(values.Length > 6 ? values[6] : null);

                                // Validación de chargeId
                                if (string.IsNullOrWhiteSpace(chargeId) || !Regex.IsMatch(chargeId, @"^[a-zA-Z0-9]+$"))
                                {
                                    if (string.IsNullOrWhiteSpace(chargeId))
                                    {
                                        errors.Add($"Invalid chargeId: {chargeId} on line: {line}. It is empty or null.");
                                    }
                                    else
                                    {
                                        errors.Add($"Invalid chargeId: {chargeId} on line: {line}. It must be alphanumeric.");
                                    }
                                    continue;
                                }

                                // Validación de nameCompany
                                if (string.IsNullOrWhiteSpace(nameCompany) || !Regex.IsMatch(nameCompany, @"^[a-zA-Z0-9 ]+$"))
                                {
                                    errors.Add($"Invalid nameCompany: {nameCompany} in chargeId: {chargeId}. It must be alphanumeric and not empty.");

                                    continue;
                                }

                                // Validación de companyId
                                if (string.IsNullOrWhiteSpace(companyId) || companyId == "*******" || !Regex.IsMatch(companyId, @"^[a-zA-Z0-9]+$"))
                                {
                                    if (string.IsNullOrWhiteSpace(companyId))
                                    {
                                        errors.Add($"Invalid companyId: {companyId} in chargeId: {chargeId}. It must not be equal to ' '.");
                                    }
                                    else if (companyId == "*******")
                                    {
                                        errors.Add($"Invalid companyId: {companyId} in chargeId: {chargeId}. It must not be equal to '*******'.");
                                    }
                                    else
                                    {
                                        errors.Add($"Invalid companyId: {companyId} in chargeId: {chargeId}. It must not contain special characters.");
                                    }
                                    continue;
                                }

                                // Validación de amount
                                if (!decimal.TryParse(values[3], NumberStyles.Any, CultureInfo.InvariantCulture, out amount) || amount <= 0)
                                {
                                    errors.Add($"Invalid amount: {values[3]} in chargeId: {chargeId}. It must be a valid decimal number greater than 0.");
                                    continue;
                                }

                                // Insertar los datos solo si todos son válidos
                                using (SqlCommand command = new SqlCommand("ImportChargesData", connection))
                                {
                                    command.CommandType = CommandType.StoredProcedure;
                                    command.Parameters.AddWithValue("@ChargeID", chargeId);
                                    command.Parameters.AddWithValue("@CompanyName", nameCompany);
                                    command.Parameters.AddWithValue("@CompanyID", companyId);
                                    command.Parameters.AddWithValue("@Amount", amount);
                                    command.Parameters.AddWithValue("@Status", status);
                                    command.Parameters.AddWithValue("@CreatedAt", createdAt);
                                    command.Parameters.AddWithValue("@UpdatedAt", (object?)updatedAt ?? DBNull.Value);

                                    await command.ExecuteNonQueryAsync();
                                }

                                successCount++;
                            }
                            catch (Exception ex)
                            {
                                // Manejo de errores
                                errors.Add($"Error on line: {line}. Details: {ex.Message}");

                            }
                        }
                    }

                    response.Success = successCount != 0;
                    response.SuccessCount = successCount;
                    response.Message = errors.Count == 0 ? "File imported successfully." : "The file was processed with errors.";

                    response.Errors = errors.ToArray();
                }
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "Error processing the file.";
                response.Errors = new[] { ex.Message };
            }

            return response;
        }
    }
}
