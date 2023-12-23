using OpenTelemetry;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Payments.Microservice;
using SharedMessages.SharedData;

var builder = Host.CreateDefaultBuilder(args);

builder.UseNServiceBus(hostBuilderContext =>
    {
        var endpointName = hostBuilderContext.Configuration.GetSection("PaymentsEndpointName").Value;
        var endpointConfiguration =
            new EndpointConfiguration(endpointName);
        endpointConfiguration.EnableOpenTelemetry();
        endpointConfiguration.UseSerialization<SystemJsonSerializer>();
        endpointConfiguration.MakeInstanceUniquelyAddressable(Environment.MachineName);

    
        //TODO move this to rabbitMQ
        endpointConfiguration.UseTransport<LearningTransport>();

       
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

var tracerProvider = Sdk.CreateTracerProviderBuilder()
    .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("PaymentsEndpoint"))
    .AddSource("NServiceBus.Core")
    .AddJaegerExporter()    .AddConsoleExporter()

    .Build();

var host = builder.Build();
host.Run();