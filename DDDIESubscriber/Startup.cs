﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using DDDEventBus;
using DDDEventBusRabbitMQ;
using DDDIESubscriber.IntegrationEvents.Events;
using DDDEventBus.Abstractions;

using DDDIESubscriber.IntegrationEvents.EventHandling;
using RabbitMQ.Client;
using Autofac;
using Autofac.Extensions.DependencyInjection;

namespace DDDIESubscriber
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            services.RegisterEventBus(Configuration);

            var container = new ContainerBuilder();
            container.Populate(services);

            return new AutofacServiceProvider(container.Build());

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseMvc();
            app.ConfigureEventBus();
        }
    }

    static class CustomExtensionsMethods
    {
        public static IServiceCollection RegisterEventBus(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<IEventBusSubscriptionsManager, InMemoryEventBusSubscriptionsManager>();

            services.AddSingleton<IRabbitMQPersistentConnection>(sp =>
            {
                var logger = sp.GetRequiredService<ILogger<DefaultRabbitMQPersistentConnection>>();
                var factory = new ConnectionFactory()
                {
                    HostName = "localhost",
                    UserName = "guest",
                    Password = "guest"

                };
                return new DefaultRabbitMQPersistentConnection(factory, logger);
            });
            services.AddSingleton<IEventBus, EventBusRabbitMQ>(sp =>
            {
                var conn = sp.GetRequiredService<IRabbitMQPersistentConnection>();
                var iLifetimeScope = sp.GetRequiredService<ILifetimeScope>();
                var logger = sp.GetRequiredService<ILogger<EventBusRabbitMQ>>();
                var subsManager = sp.GetRequiredService<IEventBusSubscriptionsManager>();

                return new EventBusRabbitMQ(conn, logger, iLifetimeScope, subsManager, 
                    queueName: "DDDIESubscriber" /* have my own queue to receive messages */);
            });

            //add integration event handlers as services
            services.AddTransient<LoanAppliedIntegrationEventHandler>();

            return services;
        }

        public static void ConfigureEventBus(this IApplicationBuilder app)
        {
            var eventBus = app.ApplicationServices.GetRequiredService<IEventBus>();
            //From Rabbit MQ perspective, this will create a binding between the exchange and the queue using LoanAppliedIntegrationEvent 
            //as the routing key.
            eventBus.Subscribe<LoanAppliedIntegrationEvent,LoanAppliedIntegrationEventHandler>();
        }


    }


}
