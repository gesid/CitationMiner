using Hangfire;
using Hangfire.Dashboard;
using Hangfire.PostgreSql;
using Microsoft.EntityFrameworkCore;
using ScrapingWashes.Context;
using ScrapingWashes.Models;
using ScrapingWashes.Repository;
using ScrapingWashes.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors();
builder.Services.AddAuthorization();

builder.Services.AddScoped<BaseModelRepository<Paper>>();
builder.Services.AddScoped<BaseModelRepository<Edition>>();
builder.Services.AddScoped<BaseModelRepository<Author>>();
builder.Services.AddScoped<BaseModelRepository<AuthorPaper>>();
builder.Services.AddScoped<ScrapingWashesService>();

builder.Services.AddDbContext<AppDbContext>(options =>
  options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddHangfire(config =>
{
    config.UsePostgreSqlStorage(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddHangfireServer();

var app = builder.Build();

app.UseRouting();

app.UseCors(option => option
    .AllowAnyOrigin()
    .AllowAnyMethod()
    .AllowAnyHeader());

app.UseAuthorization();

app.UseHangfireDashboard("", new DashboardOptions
{
    Authorization = new[] { new MyAuthorizationFilter() },
    IsReadOnlyFunc = (DashboardContext context) => false,
});

RecurringJob.AddOrUpdate<ScrapingWashesService>(
    "ScrapingWashesService",
    x => x.Init(),
    Cron.Daily(6, 0));

app.Run();

public class MyAuthorizationFilter : IDashboardAuthorizationFilter
{
    public bool Authorize(DashboardContext context)
    {
        var httpContext = context.GetHttpContext();
        return true;
    }
}