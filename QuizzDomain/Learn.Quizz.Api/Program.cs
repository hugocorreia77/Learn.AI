using Learn.AI.Client;
using Learn.Core.Api.Extensions;
using Learn.Core.Shared.Services;
using Learn.Core.Shared.Services.Abstractions;
using Learn.Quizz.Api.Extensions;
using Learn.Quizz.Services;
using Learn.Quizz.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.ConfigureLearningAuthentication()
        .ConfigureRedis()
        .ConfigureSignalR(true)
        ;


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
app.UseAuthentication();

app.MapControllers();


app.MapHub<QuizHub>("quizHub");

var consumerService = app.Services.GetRequiredService<IQuizConsumerService>();
consumerService.ListenForPlayerJoinMessage(); 


app.Run();
