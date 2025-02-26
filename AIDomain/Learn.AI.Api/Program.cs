using Learn.AI.Api.Extensions;
using Learn.Core.Api.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.AddLearningAuthentication();
builder.Services.ConfigureControllers()
                .ConfigureConventions()
                .ConfigureSwaggerGen();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.AddServices();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment() || app.Environment.IsEnvironment("DockerLocal"))
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

if (!app.Environment.IsEnvironment("DockerLocal"))
{
    app.UseHttpsRedirection();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
