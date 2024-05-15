using Azure.Storage.Blobs;
using ManeroProductsFunction.Data.Context;
using Microsoft.Azure.Functions.Worker;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureServices(services =>
    {
        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();
        services.AddDbContext<DataContext>(x => x.UseCosmos(Environment.GetEnvironmentVariable("CosmosDB")!, Environment.GetEnvironmentVariable("ManeroProducts")!));
    })

    .Build();

host.Run();
