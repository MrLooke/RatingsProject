using MediaBoard.Server.Entities;
using MediaBoard.Server.Features.Artists.SearchArtists;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: "AllowFrontEnd",
        policy =>
        {
            policy.WithOrigins("https://localhost:60302")
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
});

//Repository Classes
builder.Services.AddScoped<ISearchService, SearchService>();

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
