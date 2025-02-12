using CompanyAPI.Models;
using CompanyAPI.Services.Calculations;
using CompanyAPI.Services.DBtoCSV;
using CompanyAPI.Services.ExcelToSB;
using CompanyAPI.Services.SetOfNumber;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<CompanyChargesDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("connectionDB"))
    );

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddScoped<ICSVToDBService, CSVToDBService>();
builder.Services.AddScoped<IDBtoCSVService, DBtoCSVService>();
builder.Services.AddScoped<ISetofNumberService, SetofNumberService>();
builder.Services.AddScoped<ICalculationService, CalculationService>();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen();
var app = builder.Build();
app.Use(async (context, next) => {
    if (context.Request.Path == "/")
    {
        context.Response.Redirect("/swagger/index.html", permanent: false);
        return;
    }
    await next();
});

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(); 
    app.UseSwagger();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
