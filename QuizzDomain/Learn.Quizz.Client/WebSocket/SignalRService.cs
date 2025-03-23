using Learn.Quizz.Models.Hub;
using Learn.Quizz.Models.Messages;
using Learn.Quizz.Models.Question;
using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Threading.Tasks;

namespace Learn.Quizz.Client.WebSocket
{
    public class SignalRService
    {
        private HubConnection _hubConnection;

        public event Action<QuestionReference> OnQuestionSentMessageReceived;
        public event Action<string> OnPlayerJoinedMessageReceived;
        public event Action<string> OnPlayerUnjoinedMessageReceived;
        public event Action<string> OnQuestionSolutionSentMessageReceived;
        public event Action<StartingGame> OnGameStartingMessageReceived;
        public event Action<string> OnGameEndedMessageReceived;

        public async Task StartConnectionAsync(string url)
        {
            _hubConnection = new HubConnectionBuilder()
                .WithUrl(url)
                .WithAutomaticReconnect() 
                .Build();

            _hubConnection.On<QuestionReference>(HubMethods.ServerToClient.QuestionSent, message =>
            {
                OnQuestionSentMessageReceived?.Invoke(message);
            });

            _hubConnection.On<string>(HubMethods.ServerToClient.PlayerJoined, message =>
            {
                OnPlayerJoinedMessageReceived?.Invoke(message);
            });

            _hubConnection.On<string>(HubMethods.ServerToClient.PlayerUnjoined, message =>
            {
                OnPlayerUnjoinedMessageReceived?.Invoke(message);
            });

            _hubConnection.On<string>(HubMethods.ServerToClient.QuestionSolutionSent, message =>
            {
                OnQuestionSolutionSentMessageReceived?.Invoke(message);
            });

            _hubConnection.On<StartingGame>(HubMethods.ServerToClient.GameStarting, message =>
            {
                OnGameStartingMessageReceived?.Invoke(message);
            });

            _hubConnection.On<string>(HubMethods.ServerToClient.GameEnded, message =>
            {
                OnGameEndedMessageReceived?.Invoke(message);
            });

            await _hubConnection.StartAsync();
        }

        public async Task CloseConnectionAsync()
        {
            if (_hubConnection is not null)
            {
                await _hubConnection.StopAsync();
                await _hubConnection.DisposeAsync();
            }
        }
    }
}
