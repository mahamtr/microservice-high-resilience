using Api.Gateway.Services;
using MongoDB.Driver;
using OpenTelemetry;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using SharedMessages;
using SharedMessages.Events;
using EndpointConfiguration = NServiceBus.EndpointConfiguration;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddControllers();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<ICommandService, CommandService>();

builder.Host.UseNServiceBus(hostBuilderContext =>
{
    var mongoDbConnectionString = hostBuilderContext.Configuration.GetSection("MongoDbConnectionString").Value;
    var rabbitConnectionString = hostBuilderContext.Configuration.GetSection("RabbitConnectionString").Value;
    var endpointName = hostBuilderContext.Configuration.GetSection("ApiGatewayEndpointName").Value;
    var endpointConfiguration =
        new EndpointConfiguration(endpointName);
    endpointConfiguration.EnableOpenTelemetry();
    endpointConfiguration.UseSerialization<SystemJsonSerializer>();
    

var transport = endpointConfiguration.UseTransport<RabbitMQTransport>();
    transport.UseConventionalRoutingTopology(QueueType.Quorum);
    transport.ConnectionString(rabbitConnectionString);

    endpointConfiguration.EnableCallbacks();
    endpointConfiguration.EnableInstallers();

    // var routing = transport.Routing();
    // routing.RouteToEndpoint(typeof(StartOrder), inventoryEndpointName);
    var persistence = endpointConfiguration.UsePersistence<MongoPersistence>();
    endpointConfiguration.MakeInstanceUniquelyAddressable("MAAI");
    persistence.MongoClient(new MongoClient(mongoDbConnectionString));
    // var metrics = endpointConfiguration.EnableMetrics();
    // metrics.SendMetricDataToServiceControl("Particular.Monitoring", TimeSpan.FromMilliseconds(500));

    return endpointConfiguration;
});

var tracerProvider = Sdk.CreateTracerProviderBuilder()
    .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("ApiGatewayEndpoint"))
    .AddSource("NServiceBus.Core")
    .AddSource("*")
    .AddJaegerExporter()
    .AddConsoleExporter()
    .Build();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapControllers();
app.Run();