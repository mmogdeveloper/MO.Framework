using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MO.Algorithm.Redis;
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

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<ConsoleLifetimeOptions>(options =>
            {
                options.SuppressStatusMessages = true;
            });
            services.AddSingleton<ClusterHostedService>();
            services.AddSingleton<IHostedService>(_ => _.GetService<ClusterHostedService>());
            services.AddSingleton(_ => _.GetService<ClusterHostedService>().Client);
            services.AddControllers(options =>
            {
                options.Filters.Add<GlobalExceptionFilter>();
            });
            services.AddDbContext<MODataContext>(options =>
            {
                options.UseMySql(_configuration.GetConnectionString("MOData"));
            });
            services.AddDbContext<MORecordContext>(options =>
            {
                options.UseMySql(_configuration.GetConnectionString("MORecord"));
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env,
            MODataContext dataContext, MORecordContext recordContext)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            //初始化数据库
            dataContext.Database.Migrate();
            recordContext.Database.Migrate();

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            TokenRedis.Initialization(_configuration.GetConnectionString("TokenRedis"));
            DataRedis.Initialization(_configuration.GetConnectionString("DataRedis"));
        }
    }
}
