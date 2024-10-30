using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.OutputCaching;
using MinimalAPIsMovies.Endpoints;
using MinimalAPIsMovies.Entities;
using MinimalAPIsMovies.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Services zone - Begin

builder.Services.AddScoped<IGenresRepository, GenresRepository>();
/*
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(configuration =>
    {
        configuration.WithOrigins(builder.Configuration["AllowedOrigins"]!).AllowAnyMethod().AllowAnyHeader();
    });

    options.AddPolicy("free", configuration =>
    {
        configuration.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
    });
});
*/
builder.Services.AddOutputCache();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Services zone - End

var app = builder.Build();

// Middlewares zone - Begin

app.UseSwagger();
app.UseSwaggerUI();

//app.UseCors();
app.UseOutputCache();

app.MapGroup("/genres").MapGenres();

// Middlewares zone - End

app.Run();
