using Microsoft.OpenApi.Models;
using QTPCR.Services.Contexts;
using QTPCR.Services.Contracts;
using System;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddScoped<IChangeRequestServices, ChangeRequestServices>();
builder.Services.AddScoped<IRealisService, RealisServices>();

builder.Configuration.AddJsonFile("appsettings.json");

var app = builder.Build();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "QMR v2",
        Version = "v1",
        Description = "API Description" // Optional
    });
});


// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Your API v1");
});


app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
