using Core.Interfaces.Services;
using Infrastructure.Services;
using WorkerService;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<Worker>();

builder.Services.AddSingleton(builder.Configuration);
builder.Services.AddHttpClient<IExchangeRateService, ExchangeRateService>();
builder.Services.AddSingleton<IDatabaseService, PostgresService>();
builder.Services.AddSingleton<IIndexingService, ElasticService>();
builder.Services.AddHostedService<Worker>();


var host = builder.Build();
host.Run();
