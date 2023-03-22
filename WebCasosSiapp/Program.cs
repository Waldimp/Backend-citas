using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using WebCasosSiapp.Models.Configurations;

var builder = WebApplication.CreateBuilder(args);

// Add database context

// Add environment configurations
var jwtConfig = new JwtConfig
{
    Key = builder.Configuration["JWT:key"]
};

var environments = new EnvironmentConfig
{
    DatabaseConnection = builder.Configuration.GetConnectionString("DatabaseConnection")
};

// Add JWT configuration
var key = Encoding.UTF8.GetBytes(jwtConfig.Key);
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateAudience = false,
        ValidateIssuer = false
    };
});

// Add Interfaces
builder.Services.AddSingleton(jwtConfig);
builder.Services.AddSingleton(environments);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors(option =>
{
    option.WithOrigins("*").AllowAnyMethod().AllowAnyHeader();
});

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();