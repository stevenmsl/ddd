using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
//using RabbitMQ.Client;

namespace DDDRabbitMQ
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            services.AddRabbitMQ(Configuration);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();
        }
    }

    static class CustomExtensionsMethods
    {
        public static IServiceCollection AddRabbitMQ(this IServiceCollection services, IConfiguration configuration)
        {
            //services.AddSingleton<IRabbitMQPersistentConnection>( sp =>  
            //{
            //    var logger = sp.GetRequiredService<ILogger<MQ.DefaultConnection>>();
            //    var factory = new ConnectionFactory()
            //    {
            //        HostName = "localhost",
            //        UserName = "guest",
            //        Password = "guest"
            //    };
            //    return new MQ.DefaultConnection(factory, logger);
            //});
           

            return services;
        }
    }


}
