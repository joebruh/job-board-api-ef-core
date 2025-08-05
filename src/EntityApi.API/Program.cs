using System.Text;
using EntityApi.Data;
using EntityApi.API.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using Microsoft.Data.Sqlite;

public partial class Program {

    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.

        builder.Services.AddControllers()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
            });
        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
        builder.Services.AddOpenApi();

        if (builder.Environment.IsEnvironment("TestingSQLite"))
        {
            // Use SQLite in-memory for testing
            var connection = new SqliteConnection("Filename=:memory:");
            connection.Open();  // Important: keep open for lifetime of app

            builder.Services.AddDbContext<EntityApiDbContext>(options =>
            {
                options.UseSqlite(connection);
            });

        }
        else if (builder.Environment.IsEnvironment("TestingMSSQL"))
        {
            builder.Services.AddDbContext<EntityApiDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("Testing")));
            
        }
        else
        {
            builder.Services.AddDbContext<EntityApiDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("Default")));

        }


        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = builder.Configuration["AppSettings:Issuer"],
                    ValidateAudience = true,
                    ValidAudience = builder.Configuration["AppSettings:Audience"],
                    ValidateLifetime = true,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(builder.Configuration["AppSettings:Token"]!)
                    ),
                    ValidateIssuerSigningKey = true
                };
            });

        builder.Services.AddScoped<IAuthService, AuthService>();

        builder.Services.AddHttpContextAccessor();
        builder.Services.AddScoped<IJobPostService, JobPostService>();

        var app = builder.Build();

        // In a testing environment, refresh the database
        if (builder.Environment.IsEnvironment("TestingSQLite") || builder.Environment.IsEnvironment("TestingMSSQL"))
        {
            using (var scope = app.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<EntityApiDbContext>();
                db.Database.EnsureDeleted();
                db.Database.EnsureCreated();
            }
        }

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
            app.MapScalarApiReference();
        }

        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }


}
