using VirtualScale.Api.Extensions;
using VirtualScale.Api.Hubs;
using VirtualScale.Domain.Entities;
using VirtualScale.Worker;
using VirtualScale.Worker.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton(new CalibrationData(10, 2, 3, 10));
builder.Services.AddSingleton<Scale>();
builder.Services.AddSingleton<SerialHandler>();
builder.Services.AddHostedService<Worker>();
builder.Services.AddSignalR();

var app = builder.Build();

InitializationService.Initialize(app.Services.GetRequiredService<Scale>());

app.MapApiEndpoints();
app.MapHub<ScaleHub>("/hubs/scale");

app.Run();
