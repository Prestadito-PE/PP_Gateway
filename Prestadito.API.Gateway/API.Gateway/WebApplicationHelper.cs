using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Ocelot.DependencyInjection;
using System.Text;

namespace API.Gateway
{
    public static class WebApplicationHelper
    {
        readonly static string myCors = "myCors";

        public static WebApplication CreateWebApplication(this WebApplicationBuilder builder)
        {
            var provider = builder.Services.BuildServiceProvider();
            var configuration = provider.GetRequiredService<IConfiguration>();

            builder.Configuration.AddJsonFile("ocelot.json", false, true);
            builder.Services.AddOcelot();

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
                var urlList = configuration.GetSection("AllowedOrigin").GetChildren().Select(c => c.Value).ToArray();
                options.AddPolicy(myCors,
                    builder => builder.WithOrigins(urlList)
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials());
            });

            return builder.Build();
        }

        public static WebApplication ConfigureWebApplication(this WebApplication app)
        {
            app.UseCors(myCors);
            app.UseAuthentication();

            return app;
        }
    }
}
