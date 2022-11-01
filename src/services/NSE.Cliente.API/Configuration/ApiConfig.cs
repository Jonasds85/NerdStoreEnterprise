using Microsoft.EntityFrameworkCore;
using NSE.Cliente.API.Data;
using NSE.WebAPI.Core.Identidade;

namespace NSE.Cliente.API.Configuration
{
    public static class ApiConfig
    {
        public static void AddApiConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ClientesContext>(options => options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            services.AddControllers();

            services.AddEndpointsApiExplorer();

            services.AddCors(opt => {
                opt.AddPolicy("Total", build => build.AllowAnyOrigin()
                                                     .AllowAnyMethod()
                                                     .AllowAnyHeader());
            });
        }

        public static void UseApiConfiguration(this WebApplication app)
        {
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseCors("Total");
                        
            app.UseAuthConfiguration();

            app.MapControllers();

            app.Run();
        }
    }
}
