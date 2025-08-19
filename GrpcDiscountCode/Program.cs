using GrpcDiscountCode.Data;
using GrpcDiscountCode.Data.Repositories;
using GrpcDiscountCode.Services;
using Microsoft.EntityFrameworkCore;

namespace GrpcDiscountCode
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var path = Path.Combine(AppContext.BaseDirectory, "discount.db");
            builder.Services.AddDbContext<DiscountContext>(opt => opt.UseSqlite($"Data Source={path}"));

            builder.Services.AddTransient<IDiscountCodeRepository, DiscountCodeRepository>();
            builder.Services.AddTransient<IDiscountCodeService, DiscountCodeService>();

            // Add services to the container.
            builder.Services.AddGrpc();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            app.MapGrpcService<DiscountGrpcService>();
            app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

            app.Run();
        }
    }
}