using Microsoft.AspNetCore.Mvc;
using SigmaAssessment.Models;
using System.Reflection;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var dbConn = builder.Configuration.GetConnectionString("CandidatesDb");

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddDbContext<CandidateDbCtx>(options =>
{
    options.UseSqlServer(dbConn, options => options.EnableRetryOnFailure());
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(
    options =>
    {
        options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
        {
            Version = "v1",
            Title = "Candidates API",
            Description = "A simple API to manage candidates"
        });

        var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
        options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
        options.EnableAnnotations();
    }
);

builder.Services.Configure<ApiBehaviorOptions>(Options =>
{
    Options.SuppressModelStateInvalidFilter = true;
});

builder.Services.AddLogging();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
