using VirtualScale.Domain.Entities;
using VirtualScale.Worker;
using VirtualScale.Worker.Services;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddSingleton(new CalibrationData(1000, 1, 2, 1000));
builder.Services.AddSingleton<Scale>();
builder.Services.AddSingleton<SerialHandler>();
builder.Services.AddHostedService<Worker>();

var host = builder.Build();
InitializationService.Initialize(host.Services.GetRequiredService<Scale>());
host.Run();
