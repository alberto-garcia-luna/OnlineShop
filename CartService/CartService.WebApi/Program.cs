using CartService.Application;
using CartService.Infrastructure;
using CartService.WebApi;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddServices();
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);

var app = builder.Build();

app.ConfigureMiddleware();

app.Run();

public partial class Program { }