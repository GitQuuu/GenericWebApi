using Api.Configuration;
using Api.Extensions;
using DAL;

var builder = WebApplication.CreateBuilder(args);
MapsterConfiguration.Configure();

// Configure services
builder.Services.AddControllers();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IdentitySeeder>();
builder.Services.AddDatabaseConfiguration(builder.Configuration);
builder.Services.AddIdentityConfiguration();
builder.Services.AddApplicationServices();
builder.Services.AddJwtAuthentication(builder.Configuration, builder.Environment);
builder.Services.AddSwaggerConfiguration(builder.Configuration);

var app = builder.Build();

// Seed database
if (builder.Environment.IsDevelopment())
{
    await app.SeedDatabaseAsync();
}

// Configure middleware pipeline
app.ConfigureDevelopmentMiddleware();
app.UseSwaggerConfiguration();
app.ConfigureRequestPipeline();

app.Run();