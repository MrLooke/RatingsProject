using MediaBoard.Server.Entities;
using MediaBoard.Server.Features.Artists.SearchArtists;
using MediaBoard.Server.Features.Artists.ArtistPage;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var allowedOrigin = builder.Configuration["ALLOWED_ORIGIN"];
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: "AllowFrontEnd",
        policy =>
        {
            policy.WithOrigins(allowedOrigin)
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
});

//Repository Classes
builder.Services.AddScoped<ISearchService, SearchService>();
builder.Services.AddScoped<IArtistService, ArtistService>();

//DB Context
builder.Services.AddDbContextPool<AppDbContext>(opt => opt.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

app.UseRouting();
app.UseCors("AllowFrontEnd");

app.UseDefaultFiles();
app.MapStaticAssets();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.MapFallbackToFile("/index.html");

app.Run();
