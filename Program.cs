using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using QTPCR.Services.Contexts;
using QTPCR.Services.Contracts;
using System;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddScoped<IChangeRequestServices, ChangeRequestServices>();
builder.Services.AddScoped<IRealisService, RealisServices>();
builder.Services.AddScoped<IQtpServices, QtpServices>();
builder.Services.AddScoped<ILogsServices, LogsServices>();
builder.Services.AddScoped<ITokenServices, TokenServices>();

builder.Configuration.AddJsonFile("appsettings.json");

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "QTP Change Request",
        Version = "v1",
        Description = "API Description" // Optional
    });
});

var app = builder.Build();

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
