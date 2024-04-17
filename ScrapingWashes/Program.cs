using Microsoft.EntityFrameworkCore;
using ScrapingWashes.Context;
using ScrapingWashes.Scraping;
using ScrapingWashes.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
  options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<EditionService>();

var app = builder.Build();

app.Run();