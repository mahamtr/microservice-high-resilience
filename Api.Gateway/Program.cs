using Microsoft.AspNetCore.Server.Kestrel;
using NServiceBus;
using SharedMessages;
using EndpointConfiguration = NServiceBus.EndpointConfiguration;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddControllers();
builder.Services.AddSwaggerGen();

builder.Host.UseNServiceBus(hostBuilderContext =>
{
    var endpointName = hostBuilderContext.Configuration.GetSection("ApiGatewayEndpointName").Value;
    var inventoryEndpointName = hostBuilderContext.Configuration.GetSection("InventoryEndpointName").Value;
    var endpointConfiguration =
        new EndpointConfiguration(endpointName);
    endpointConfiguration.UseSerialization<SystemJsonSerializer>();
    
    
    //TODO move this to rabbitMQ
    var transport = endpointConfiguration.UseTransport<LearningTransport>();

    var routing = transport.Routing();
    routing.RouteToEndpoint(typeof(PlaceOrderCommand), inventoryEndpointName);
    // var metrics = endpointConfiguration.EnableMetrics();
    // metrics.SendMetricDataToServiceControl("Particular.Monitoring", TimeSpan.FromMilliseconds(500));

    return endpointConfiguration;
});


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