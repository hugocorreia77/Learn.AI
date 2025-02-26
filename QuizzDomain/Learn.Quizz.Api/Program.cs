using Learn.Core.Api.Extensions;
using Learn.Quizz.Api.Extensions;
using Learn.AI.Client;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.AddLearningAuthentication();
builder.Services.ConfigureControllers()
                .ConfigureConventions()
                .ConfigureSwaggerGen();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.AddRepositories()
        .AddServices();

builder.Services.AddAIClient(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
