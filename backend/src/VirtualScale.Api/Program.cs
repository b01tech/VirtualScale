using Microsoft.EntityFrameworkCore;
using VirtualScale.Api.Extensions;
using VirtualScale.Api.Hubs;
using VirtualScale.Api.Infrastructure.Persistence;
using VirtualScale.Api.Infrastructure.Repositories;
using VirtualScale.Api.Infrastructure.Services;
using VirtualScale.Domain.Entities;
using VirtualScale.Worker;
using VirtualScale.Worker.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDatabaseContext(builder.Environment);

builder.Services.AddScoped<ICalibrationRepository, CalibrationRepository>();
builder.Services.AddScoped<CalibrationPersistenceService>();

builder.Services.AddSingleton(new CalibrationData(10, 2, 3, 10));
builder.Services.AddSingleton<Scale>();
builder.Services.AddSingleton<SerialHandler>();
builder.Services.AddHostedService<Worker>();
builder.Services.AddHostedService<SerialConnectionWorker>();
builder.Services.AddSignalR();
builder.Services.AddCorsPolicy();

var app = builder.Build();

await app.LoadDatabaseAsync();

app.MapApiEndpoints();
app.UseCorsPolicy();
app.MapAppHubs();

app.Run();
