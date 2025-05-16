using GitHubPortfolio.Service.Cache;
using GitHubPortfolio.Service.Models;
using GitHubPortfolio.Service.Services;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "GitHub Portfolio API",
        Version = "v1",
        Description = "API for retrieving GitHub portfolio information"
    });
});

// Add memory cache
builder.Services.AddMemoryCache();
builder.Services.AddSingleton<ICacheService, InMemoryCacheService>();

// Add GitHub service
builder.Services.AddScoped<IGitHubService, GitHubService>();

// Configure GitHub options
builder.Services.Configure<GitHubPortfolioOptions>(
    builder.Configuration.GetSection(GitHubPortfolioOptions.SectionName));

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors();
app.UseAuthorization();
app.MapControllers();

app.Run();