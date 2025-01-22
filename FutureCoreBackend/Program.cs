
using FutureCoreBackend.AutoMapper;
using FutureCoreBackend.Models;
using FutureCoreBackend.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using System.Text.Json.Serialization;

namespace FutureCoreBackend
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
                });        
            
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            //DbContext
            builder.Services.AddDbContext<Context>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultDB"))
                .LogTo(Console.WriteLine, LogLevel.Information);
            });

            //AutoMapper
            builder.Services.AddAutoMapper(
                typeof(EmployeeProfile),
                typeof(FamilyMemberProfile));

            //CORS
            builder.Services.AddCors(options =>
                options.AddPolicy("default", builder =>
                    builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()));

            //SwaggerConfig
            builder.Services.AddSwaggerGen(opt =>
            {
                opt.EnableAnnotations();

                opt.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "ASP.NET 8 Web API",
                    Description = "Future core API Project"
                });
            });

            //register unit of work
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
            builder.Services.AddScoped<IFamilyMemberRepo, FamilyMemberRepo>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseCors("default");

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
