using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using System.Text;

string myCors = "myCors";
var builder = WebApplication.CreateBuilder(args);

if (builder.Environment.IsDevelopment())
{
    builder.Configuration.AddJsonFile("ocelot.Development.json", optional: false, reloadOnChange: true);
}
else
{
    builder.Configuration.AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);
}
builder.Services.AddOcelot(builder.Configuration);

//Authentication
var secretKey = Encoding.UTF8.GetBytes("8yBEHrPo5rut8alxAWnGd2nvZr4u7xeThWm2Z00q4K2bPeShVm");

builder.Services.AddAuthentication(config =>
{
    config.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    config.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        ValidateIssuer = false,
        ValidateAudience = false,
        IssuerSigningKey = new SymmetricSecurityKey(secretKey),
        ClockSkew = new TimeSpan(0)
    };
});

builder.Services.AddCors(options =>
{
    var urlList = builder.Configuration.GetSection("AllowedOrigin").GetChildren().Select(c => c.Value).ToArray();
    options.AddPolicy(myCors,
        builder => builder.WithOrigins(urlList)
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials());
});

var app = builder.Build();

app.MapControllers();
app.UseAuthentication();
app.UseCors(myCors);
await app.UseOcelot();

app.Run();
