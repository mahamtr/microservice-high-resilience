using MongoDB.Driver;
using Orchestrator.Microservice;


var builder = Host.CreateDefaultBuilder(args);

builder.UseNServiceBus(hostBuilderContext =>
    {
        var mongoDbConnectionString = hostBuilderContext.Configuration.GetSection("MongoDbConnectionString").Value;
        var endpointName = hostBuilderContext.Configuration.GetSection("OrchestratorEndpointName").Value;
        var endpointConfiguration =
            new EndpointConfiguration(endpointName);
        endpointConfiguration.UseSerialization<SystemJsonSerializer>();
    
    
        //TODO move this to rabbitMQ
        endpointConfiguration.UseTransport<LearningTransport>();

        var persistence = endpointConfiguration.UsePersistence<MongoPersistence>();
        persistence.MongoClient(new MongoClient(mongoDbConnectionString));
        // var metrics = endpointConfiguration.EnableMetrics();
        // metrics.SendMetricDataToServiceControl("Particular.Monitoring", TimeSpan.FromMilliseconds(500));
        endpointConfiguration.MakeInstanceUniquelyAddressable(Environment.MachineName);
        return endpointConfiguration;
    })
    .ConfigureServices((context, services) =>
    {
        services.AddHostedService<Worker>();
    });


var host = builder.Build();
host.Run();