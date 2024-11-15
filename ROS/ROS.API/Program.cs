using Microsoft.EntityFrameworkCore;
using ROS.Implement.Repository;
using ROS.Model.Tables;
using System.Text.Json.Serialization;
namespace ROS.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAllOrigins", builder =>
                    builder.AllowAnyOrigin()
                           .AllowAnyHeader()
                           .AllowAnyMethod());
            });
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
            builder.Services.AddDbContext<ROS.Model.Tables.ApplicationDbContext>(options =>
                options.UseMySql(connectionString, new MySqlServerVersion(new Version(9, 0, 1)))); 
            builder.Services.AddControllers();

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddScoped<IAuthRepository, AuthRepository>();
            var app = builder.Build();
            app.UseCors("AllowAllOrigins");
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
