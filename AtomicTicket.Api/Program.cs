using AtomicTicket.Api;
using AtomicTicket.Application;
using AtomicTicket.Infrastructure;
using Carter;

var builder = WebApplication.CreateBuilder(args);
{
    builder.Services
        .AddPresentationLayer()
        .AddApplication()
        .AddInfrastructure(builder.Configuration);
}


builder.Services.AddControllers();

builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "AtomicTicket API V1");
        c.RoutePrefix = "swagger";
    });
    app.MapOpenApi();
}

app.MapCarter();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
