// See https://aka.ms/new-console-template for more information

using Learn.Quizz.Models.Messages;
using Learn.Quizz.Models.Question;
using Learn.Quizz.Models.Quiz.Input;
using Learn.Security.Client;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;

/// AUTH 
internal class Program
{
    private static async Task Main(string[] args)
    {
        bool loginDone = false;
        string jwt = string.Empty;

        while (!loginDone)
        {
            Console.WriteLine($"username:");
            var username = Console.ReadLine();
            Console.WriteLine($"password:");
            var password = Console.ReadLine();

            using ILoggerFactory loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
            ILogger<LearnSecurityClient> loggerSec = loggerFactory.CreateLogger<LearnSecurityClient>();

            LearnSecurityClient lsc = new LearnSecurityClient(
                loggerSec,
                new HttpClient
                {
                    BaseAddress = new Uri("https://learning-sec-api-prd-452013520192.europe-southwest1.run.app")
                });
            var authResponse = lsc.AuthenticateAsync(new Learn.Security.Models.Input.AuthenticationInput
            {
                Username = username,
                Password = password
            }, CancellationToken.None)
                .GetAwaiter()
                .GetResult();

            if (!authResponse.Success)
            {
                Console.WriteLine("auth failed");
            }
            else
            {
                loginDone = true;
                jwt = authResponse.Data;
            }
        }

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
        
        connection.On<string>("PlayerUnjoined", message =>
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
                    case "0":
                    case "1":
                    case "2":
                    case "3":

                        if (int.TryParse(message, out var index))
                        {
                            var option = currentOptions[index];

                            var input = new AnswerInput
                            {
                                QuizId = currentQuizId.Value,
                                QuestionId = currentQuestionId.Value,
                                AttemptId = option.Id
                            };

                            //await connection.SendAsync("RespondeOption", input);
                            //await connection.SendAsync("RespondeCena", input);
                            //await connection.SendAsync("cenass", input);
                            //await connection.SendAsync("Tau", 1);
                            await connection.InvokeAsync("Xau", "aaa");
                            answered = true;
                        }

                        break;
                    default:

                        //if (int.TryParse(message, out var index))
                        //{
                        //    var option = currentOptions[index];

                        //    var input = new AnswerInput
                        //    {
                        //        QuizId = currentQuizId.Value,
                        //        QuestionId = currentQuestionId.Value,
                        //        AttemptId = option.Id
                        //    };

                        //    //await connection.SendAsync("RespondeOption", input);
                        //    //await connection.SendAsync("RespondeCena", input);
                        //    //await connection.SendAsync("cenass", input);
                        //    //await connection.SendAsync("Tau", 1);
                        //    await connection.SendAsync("Xau", "aaa");
                        //    answered = true;
                        //}

                        break;
                }

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
    }
}