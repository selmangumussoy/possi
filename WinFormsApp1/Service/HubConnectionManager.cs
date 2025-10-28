using Microsoft.AspNetCore.SignalR.Client;

namespace WinFormsApp1.Service
{
    public static class HubConnectionManager
    {
        public static HubConnection HubConnection { get; private set; }

        public static async Task ConnectAsync(string token, string hubUrl)
        {
            // URL dışarıdan veriliyor (txtHubUrl.Text.Trim())
            HubConnection = new HubConnectionBuilder()
                .WithUrl($"{hubUrl}", options =>
                {
                    options.AccessTokenProvider = () => Task.FromResult(token);
                })
                .WithAutomaticReconnect()
                .Build();

            // Server tarafı 2 parametre gönderiyor (toUser, messageJson)
            HubConnection.On<string, string>("ReceiveMessage", (toUser, messageJson) =>
            {
                MessageReceived?.Invoke(toUser, messageJson);
            });

            await HubConnection.StartAsync();
        }

        // Mesajları dışarıya event olarak açıyoruz
        public static event Action<string, string> MessageReceived;
    }
}