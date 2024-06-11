using Shop.Api.Controllers;
using Shop.Infrastructure.IoC;
using Shop.Infrastructure.Extensions;

var builder = WebApplication.CreateSlimBuilder(args);

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddMassTransitServices(builder.Configuration);

builder.Services.AddSwaggerDocs();

builder.Services.AddInfraModules(builder.Configuration);

var app = builder.Build();

app.ApplyMigrations();

ProductController.MapRoutes(app);

app.AddSwaggerEndpoints();

app.Run();
