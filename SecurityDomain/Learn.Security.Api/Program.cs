using Learn.Core.Api.Extensions;
using Learn.Core.Shared.Http;
using Learn.Security.Api.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.ConfigureControllers()
                .ConfigureConventions();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.AddRepositories()
        .AddServices();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment() || app.Environment.IsEnvironment("DockerLocal"))
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(CorsPolicyHelper.CorsPolicyName); // Aplicar CORS antes do SignalR

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
