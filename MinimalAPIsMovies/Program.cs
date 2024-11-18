using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using MinimalAPIsMovies.Endpoints;
using MinimalAPIsMovies.Entities;
using MinimalAPIsMovies.Repositories;
using MinimalAPIsMovies.Services;
using MinimalAPIsMovies.Utilities;

var builder = WebApplication.CreateBuilder(args);

// Services zone - Begin
builder.Services.AddTransient<IUserStore<IdentityUser>, UserStore>();
builder.Services.AddIdentityCore<IdentityUser>();
builder.Services.AddTransient<SignInManager<IdentityUser>>();
builder.Services.AddOutputCache();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IGenresRepository, GenresRepository>();
builder.Services.AddScoped<IActorsRepository, ActorsRepository>();
builder.Services.AddScoped<IMoviesRepository, MoviesRepository>();
builder.Services.AddScoped<ICommentsRepository, CommentsRepository>();
builder.Services.AddScoped<IErrorsRepository, ErrorsRepository>();
builder.Services.AddScoped<IUsersRepository, UsersRepository>();

builder.Services.AddTransient<IFileStorage, AzureFileStorage>();
builder.Services.AddTransient<IUsersService, UsersService>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddAutoMapper(typeof(Program));
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

builder.Services.AddProblemDetails();
builder.Services.AddAuthentication().AddJwtBearer(options =>
{
    options.MapInboundClaims = false;

    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ClockSkew = TimeSpan.Zero,
        IssuerSigningKeys = KeysHandler.GetAllKeys(builder.Configuration)
        //IssuerSigningKey = KeysHandler.GetKey(builder.Configuration).First()
    };
});
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("isadmin", policy => policy.RequireClaim("isadmin"));
});
// Services zone - End

var app = builder.Build();

// Middlewares zone - Begin
app.UseSwagger();
app.UseSwaggerUI();
app.UseExceptionHandler(exceptionHandlerApp => exceptionHandlerApp.Run(async context =>
{
    var exceptionHandlerFeature = context.Features.Get<IExceptionHandlerFeature>();
    var exception = exceptionHandlerFeature?.Error!;

    var error = new Error();
    error.Date = DateTime.UtcNow;
    error.ErrorMessage = exception.Message;
    error.StackTrace = exception.StackTrace;

    var repository = context.RequestServices.GetRequiredService<IErrorsRepository>();
    await repository.Create(error);

    await Results.BadRequest(new
    {
        type = "error",
        message = "an unexpected exception has ocurred",
        status = 500
    }).ExecuteAsync(context);
}));
app.UseStatusCodePages();
app.UseOutputCache();
app.UseAuthorization();

app.MapGet("/error", () =>
{
    throw new InvalidOperationException("Example error");
});

app.MapGroup("/genres").MapGenres();
app.MapGroup("/actors").MapActors();
app.MapGroup("/movies").MapMovies();
app.MapGroup("/movies/{movieId:int}/comments").MapComments();
app.MapGroup("/users").MapUsers();
// Middlewares zone - End

app.Run();
