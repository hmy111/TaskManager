using Autofac;


namespace TaskTips.Extension
{
    public class ApplicationModule : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            //builder.RegisterType<A>().As<IA>()
            //    .InstancePerLifetimeScope();
            //builder.RegisterType<A>().As<IA>().PropertiesAutowired()
            //    .InstancePerLifetimeScope();
            //builder.RegisterType<Log.Logger>().As<ILogger>().PropertiesAutowired()
            //    .InstancePerLifetimeScope();
            //builder.RegisterType<ScopeService>()
            //      .As<IScopeService>()
            //      .InstancePerLifetimeScope();

        }

    }
}
