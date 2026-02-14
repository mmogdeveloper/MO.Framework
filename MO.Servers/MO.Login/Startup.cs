using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MO.Login.Filters;
using MO.Model.Context;

namespace MO.Login
{
    public class Startup
    {
        private IConfiguration _configuration;
        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<ConsoleLifetimeOptions>(options =>
            {
                options.SuppressStatusMessages = true;
            });
            services.AddControllers(options =>
            {
                options.Filters.Add<GlobalExceptionFilter>();
            });

            var moDataConn = _configuration.GetConnectionString("MOData");
            services.AddDbContext<MODataContext>(options =>
            {
                options.UseMySql(moDataConn, ServerVersion.AutoDetect(moDataConn));
            });

            var moRecordConn = _configuration.GetConnectionString("MORecord");
            services.AddDbContext<MORecordContext>(options =>
            {
                options.UseMySql(moRecordConn, ServerVersion.AutoDetect(moRecordConn));
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env,
            MODataContext dataContext, MORecordContext recordContext)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
