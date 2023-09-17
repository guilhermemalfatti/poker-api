
using FortisService.DataContext;
using Microsoft.EntityFrameworkCore;

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

            builder.Services.AddControllers();
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