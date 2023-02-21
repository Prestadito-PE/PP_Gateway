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
            builder.Services.AddOcelot(builder.Configuration);

            //Authentication
            var secretKey = Encoding.UTF8.GetBytes("8yBEHrPo5rut8alxAWnGd2nvZr4u7xeThWm2Z00q4K2bPeShVm");

            //builder.Services.AddAuthentication(config =>
            //{
            //    config.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            //    config.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            //}).AddJwtBearer(config =>
            //{
            //    config.RequireHttpsMetadata = false;
            //    config.SaveToken = true;
            //    config.TokenValidationParameters = new TokenValidationParameters
            //    {
            //        ValidateIssuerSigningKey = true,
            //        IssuerSigningKey = new SymmetricSecurityKey(secretKey),
            //        ValidateIssuer = false,
            //        ValidateAudience = false
            //    };
            //});
            //builder.Services.AddAuthorization();

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

            return app;
        }
    }
}
