using Hangfire;
using Microsoft.EntityFrameworkCore;
using ScrapingWashes.Context;
using ScrapingWashes.Models;
using ScrapingWashes.Repository;
using ScrapingWashes.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<SeleniumService>();
builder.Services.AddScoped<BaseModelRepository<Paper>>();
builder.Services.AddScoped<BaseModelRepository<Edition>>();
builder.Services.AddScoped<BaseModelRepository<Author>>();
builder.Services.AddScoped<BaseModelRepository<AuthorPaper>>();

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