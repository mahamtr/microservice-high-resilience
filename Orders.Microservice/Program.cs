using Orders.Microservice;
using SharedMessages.SharedData;

var builder = Host.CreateDefaultBuilder(args);

builder.UseNServiceBus(hostBuilderContext =>
    {
        var endpointName = hostBuilderContext.Configuration.GetSection("OrdersEndpointName").Value;
        var endpointConfiguration =
            new EndpointConfiguration(endpointName);
        endpointConfiguration.UseSerialization<SystemJsonSerializer>();
    
    
        //TODO move this to rabbitMQ
        endpointConfiguration.UseTransport<LearningTransport>();
        endpointConfiguration.MakeInstanceUniquelyAddressable(Environment.MachineName);

       
        // var metrics = endpointConfiguration.EnableMetrics();
        // metrics.SendMetricDataToServiceControl("Particular.Monitoring", TimeSpan.FromMilliseconds(500));

        return endpointConfiguration;
    })
    .ConfigureServices((context, services) =>
    {
        services.AddHostedService<Worker>();
        services.Configure<DatabaseSettings>(
            context.Configuration.GetSection("DatabaseConfig"));
        services.AddSingleton(typeof(IMongoRepository<>), typeof(MongoRepository<>));
    });
var host = builder.Build();
host.Run();