using Inventory.Microservice;
using OpenTelemetry;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using SharedMessages.SharedData;

var builder = Host.CreateDefaultBuilder(args);

builder.UseNServiceBus(hostBuilderContext =>
    {
        var rabbitConnectionString = hostBuilderContext.Configuration.GetSection("RabbitConnectionString").Value;
        var endpointName = hostBuilderContext.Configuration.GetSection("InventoryEndpointName").Value;
        var endpointConfiguration =
            new EndpointConfiguration(endpointName);
        endpointConfiguration.EnableOpenTelemetry();
        endpointConfiguration.UseSerialization<SystemJsonSerializer>();
    
    
    var transport = endpointConfiguration.UseTransport<RabbitMQTransport>();
    transport.UseConventionalRoutingTopology(QueueType.Quorum);
        transport.ConnectionString(rabbitConnectionString);
        endpointConfiguration.EnableInstallers();

       
        // var metrics = endpointConfiguration.EnableMetrics();
        // metrics.SendMetricDataToServiceControl("Particular.Monitoring", TimeSpan.FromMilliseconds(500));
        endpointConfiguration.MakeInstanceUniquelyAddressable("MAAI");
        return endpointConfiguration;
    })
    .ConfigureServices((context, services) =>
{
    services.AddHostedService<Worker>();
    services.Configure<DatabaseSettings>(
        context.Configuration.GetSection("DatabaseConfig"));
    services.AddSingleton(typeof(IMongoRepository<>), typeof(MongoRepository<>));
});

var tracerProvider = Sdk.CreateTracerProviderBuilder()
    .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("InventoryEndpoint"))
    .AddSource("NServiceBus.Core")
    .AddJaegerExporter()
    .AddConsoleExporter()
    .Build();

var host = builder.Build();
host.Run();