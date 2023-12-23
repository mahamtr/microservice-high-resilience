using Inventory.Microservice;
using OpenTelemetry;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using SharedMessages.SharedData;

var builder = Host.CreateDefaultBuilder(args);

builder.UseNServiceBus(hostBuilderContext =>
    {
        var endpointName = hostBuilderContext.Configuration.GetSection("InventoryEndpointName").Value;
        var endpointConfiguration =
            new EndpointConfiguration(endpointName);
        endpointConfiguration.EnableOpenTelemetry();
        endpointConfiguration.UseSerialization<SystemJsonSerializer>();
    
    
        //TODO move this to rabbitMQ
        endpointConfiguration.UseTransport<LearningTransport>();

       
        // var metrics = endpointConfiguration.EnableMetrics();
        // metrics.SendMetricDataToServiceControl("Particular.Monitoring", TimeSpan.FromMilliseconds(500));
        endpointConfiguration.MakeInstanceUniquelyAddressable(Environment.MachineName);
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