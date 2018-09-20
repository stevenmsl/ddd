using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac;

namespace DDDAutofac
{
    public class AppModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.Register(c => new SingletonQueries())
                .As<SingletonQueries>()
                .SingleInstance(); 

            builder.Register(c => new TransientQueries())
                .As<TransientQueries>()
                .InstancePerLifetimeScope();

        }

    }
}
