using Microsoft.AspNetCore.SignalR.Client;

namespace WinFormsApp1.Service
{
    public static class HubConnectionManager
    {
        public static HubConnection OrderHub { get; private set; }

        public static async Task ConnectAsync(string token, string hubUrl)
        {
            OrderHub = new HubConnectionBuilder()
                .WithUrl(hubUrl, options =>
                {
                    options.AccessTokenProvider = () => Task.FromResult(token);
                })
                .WithAutomaticReconnect()
                .Build();

            await OrderHub.StartAsync();
        }
    }
}