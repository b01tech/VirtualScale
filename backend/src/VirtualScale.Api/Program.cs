using VirtualScale.Api.Extensions;
using VirtualScale.Domain.Entities;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<Scale>();

var app = builder.Build();

app.MapApiEndpoints();

app.Run();
