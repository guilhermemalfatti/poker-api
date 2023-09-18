
using FluentValidation.AspNetCore;
using FortisService.DataContext;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Data.Entity;
using System.Reflection;

namespace FortisPokerCard.WebService
{
    public class Program
    {
        /// <summary>
        /// Entry class of the web api
        /// </summary>
        /// <param name="args"></param>
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers().AddFluentValidation(options =>
            {
                // Validate child properties and root collection elements
                options.ImplicitlyValidateChildProperties = true;
                options.ImplicitlyValidateRootCollectionElements = true;

                // Automatic registration of validators in assembly
                options.RegisterValidatorsFromAssembly(Assembly.GetExecutingAssembly());
            }); ;

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            

            //builder.Logging.ClearProviders();
            builder.Configuration.AddEnvironmentVariables("FORTIS_");
            /*builder.Host.UseSerilog((context, configuration) => {
                configuration.ReadFrom.Configuration(context.Configuration);
                configuration.Enrich.FromLogContext();
                });*/

            //DB config
            string databaseType = builder.Configuration.GetValue<string>("DatabaseType");
            _ = builder.Services.AddDbContext<FortisDbContext>(options =>
            {
                if (databaseType.ToLower() == "SQLite")
                {
                    options.UseSqlite();
                }
                else
                {
                    // todo
                }
            });


            var app = builder.Build();

            // ensure DB is created, don't use this with EF migrations
            using var scope = app.Services.CreateScope();
            using var context = scope.ServiceProvider.GetService<FortisDbContext>();
                context.Database.EnsureCreated();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }

    }
}