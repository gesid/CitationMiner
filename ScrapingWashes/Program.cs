using Hangfire;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using ScrapingWashes.Context;
using ScrapingWashes.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<SeleniumService>();
builder.Services.AddScoped<EditionService>();

builder.Services.AddDbContext<AppDbContext>(options =>
  options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddHangfire(config =>
{
    config.UseSqlServerStorage(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddHangfireServer();

var app = builder.Build();

app.UseHangfireDashboard("");

RecurringJob.AddOrUpdate<SeleniumService>(
    "SeleniumService",
    x => x.Init(),
    Cron.Daily(6, 0));

app.Run();