// See https://aka.ms/new-console-template for more information

using Learn.Quizz.Models.Messages;
using Learn.Quizz.Models.Question;
using Microsoft.AspNetCore.SignalR.Client;

string jwt = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJhYzViZjk2Yi02NWNhLTQ3YzMtYWJkOS1lY2VhMDhkMTllMjYiLCJ1bmlxdWVfbmFtZSI6InJpdGEiLCJqdGkiOiI0YWZhMmJkZC1kMzhhLTRjNDMtOTkzNi01NTE4Y2IyMmEyYWIiLCJuYW1lIjoicml0YSIsImV4cCI6MTc0MTA2NDE3OSwiaXNzIjoibGVhcm4uY29tIiwiYXVkIjoibGVhcm4uY29tIn0.80CiMbJgzfup_7Uj66RGqdgdsTO0TeZDE0Il6M1GPEA";

string hubUrl = $"https://localhost:7274/quizHub?access_token={jwt}"; // Substitua pela URL do seu SignalR Hub

List<QuestionOptionReference> currentOptions = [];
Guid? currentQuizId = null;
Guid? currentQuestionId = null;
bool answered = false;

// Criação da conexão SignalR
var connection = new HubConnectionBuilder()
    // Defina a URL do seu SignalR Hub
    .WithUrl(hubUrl) 
    .Build();

// Evento quando uma mensagem é recebida do servidor
connection.On<QuestionReference>("QuestionSent", message =>
{
    answered = false;
    Console.WriteLine();
    Console.WriteLine($"Question");
    Console.WriteLine($"Category: {message.Category}");
    Console.WriteLine($"{message.QuestionText}");

    currentQuestionId = message.Id;
    currentOptions = message.Options ?? [];

    foreach (var item in message.Options ?? [])
    {
        Console.WriteLine($"[ ] {item.Text}");
    }

});

connection.On<string>("PlayerJoined", message =>
{
    Console.WriteLine($"{message}");
});

connection.On<string>("QuestionSolutionSent", message =>
{
    Console.WriteLine($"Solution: {message}");
});

connection.On<StartingGame>("GameStarting", message =>
{
    Console.WriteLine($"{message}");
    currentQuizId = message.QuizId;
});

connection.On<string>("GameEnded", message =>
{
    Console.WriteLine($"{message}");
});


// Conectando-se ao Hub
try
{
    await connection.StartAsync();
    Console.WriteLine("Conectado ao SignalR Hub");

    // Enviar uma mensagem para o Hub
    Console.WriteLine("Opções:");
    Console.WriteLine("Exit - para sair ");
    Console.WriteLine("join - juntar ao jogo ");
    Console.WriteLine("start - iniciar o jogo");

    while (true)
    {
        var message = Console.ReadLine();
        if (message == "exit")
        {
            break;
        }

        switch (message)
        {
            case "exit":
                break;
            case "join":
                await connection.SendAsync("JoinGame", "b423e8");
                break;
            case "start":
                await connection.SendAsync("StartGame", "b423e8");
                break;
            default:

                if(int.TryParse(message, out var index))
                {
                    var option = currentOptions[index];
                    await connection.SendAsync("SetAttempt", currentQuizId, currentQuestionId, option.Id);
                    answered = true;
                }

                break;
        }

        // Envia a mensagem para o servidor SignalR
        // await connection.SendAsync("JoinGame", "b423e8");
    }
}
catch (Exception ex)
{
    Console.WriteLine($"Erro ao conectar: {ex.Message}");
    Console.ReadKey();
}
finally
{
    // Fechar a conexão ao final
    await connection.StopAsync();
    await connection.DisposeAsync();
    Console.WriteLine("Conexão encerrada");
    Console.ReadKey();
}

