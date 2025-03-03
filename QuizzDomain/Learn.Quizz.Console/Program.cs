// See https://aka.ms/new-console-template for more information

using Microsoft.AspNetCore.SignalR.Client;

string jwt = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJhYzViZjk2Yi02NWNhLTQ3YzMtYWJkOS1lY2VhMDhkMTllMjYiLCJ1bmlxdWVfbmFtZSI6InJpdGEiLCJqdGkiOiI0YWZhMmJkZC1kMzhhLTRjNDMtOTkzNi01NTE4Y2IyMmEyYWIiLCJuYW1lIjoicml0YSIsImV4cCI6MTc0MTA2NDE3OSwiaXNzIjoibGVhcm4uY29tIiwiYXVkIjoibGVhcm4uY29tIn0.80CiMbJgzfup_7Uj66RGqdgdsTO0TeZDE0Il6M1GPEA";

string hubUrl = $"https://localhost:7274/quizHub?access_token={jwt}"; // Substitua pela URL do seu SignalR Hub

// Criação da conexão SignalR
var connection = new HubConnectionBuilder()
    // Defina a URL do seu SignalR Hub
    .WithUrl(hubUrl) 
    .Build();

// Evento quando uma mensagem é recebida do servidor
connection.On<string>("ReceieMessage", message =>
{
    Console.WriteLine($"Mensagem recebida: {message}");
});

connection.On<string>("PlayerJoined", message =>
{
    Console.WriteLine($"player joined: {message}");
});


// Conectando-se ao Hub
try
{
    await connection.StartAsync();
    Console.WriteLine("Conectado ao SignalR Hub");

    // Enviar uma mensagem para o Hub
    while (true)
    {
        Console.Write("Digite uma mensagem (ou 'exit' para sair): ");
        var message = Console.ReadLine();
        if (message == "exit")
        {
            break;
        }

        // Envia a mensagem para o servidor SignalR
        await connection.SendAsync("JoinGame", "b423e8");
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

