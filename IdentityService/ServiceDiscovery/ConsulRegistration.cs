namespace IdentityService.ServiceDiscovery{
    public static class ConsulRegistration
    {
        public static IServiceCollection ConfigureConsul(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<IConsulClient, ConsulClient>(p => new ConsulClient(consulConfig =>
            {
                var address = configuration["ConsulConfig:Address"];
                consulConfig.Address = new Uri(address);
            }));
            return services;
        }

        public static IApplicationBuilder RegisterWithConsul(this IApplicationBuilder app, IHostApplicationLifetime lifetime)
        {
            var consulClient = app.ApplicationServices.GetRequiredService<IConsulClient>();
            var loggingFactory = app.ApplicationServices.GetRequiredService<ILoggerFactory>();
            var logger = loggingFactory.CreateLogger<IApplicationBuilder>();

            var task = Task.Run(async () =>
            {
                await Task.Delay(500);

                var features = app.ServerFeatures;
                var addresses = features.Get<IServerAddressesFeature>();
                var address = addresses.Addresses.FirstOrDefault();

                if (address == null)
                {
                    logger.LogError("No server address found.");
                    return;
                }

                var uri = new Uri(address); 

                var registration = new AgentServiceRegistration()
                {
                    ID = $"HospitalService",
                    Name = "HospitalService",
                    Address = uri.Host,
                    Port = uri.Port,
                    Tags = new[] { "Hospital Service", "Hospital" }
                };

                logger.LogInformation("Registering with Consul");
                consulClient.Agent.ServiceDeregister(registration.ID).Wait();
                consulClient.Agent.ServiceRegister(registration).Wait();

                lifetime.ApplicationStopping.Register(() =>
                {
                    logger.LogInformation("Deregistering from Consul");
                    consulClient.Agent.ServiceDeregister(registration.ID).Wait();
                });
            });

            return app;
        }

    }

}
