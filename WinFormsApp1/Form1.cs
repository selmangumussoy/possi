using Microsoft.AspNetCore.SignalR.Client;
using WinFormsApp1.Models;
using WinFormsApp1.Service;

namespace WinFormsApp1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        // Web login (token alma)
        private async void btnLogin_Click(object sender, EventArgs e)
        {
            try
            {
                lblStatus.Text = "🔄 Web API’ye giriş yapılıyor...";

                var authService = new AuthService(txtBaseUrl.Text); 
                var result = await authService.LoginAsync(txtWebEmail.Text, txtWebPassword.Text);

                if (result == null || string.IsNullOrEmpty(result.Jwttoken))
                {
                    lblStatus.Text = "❌ Web login başarısız.";
                    return;
                }

                AppSession.JwtToken = result.Jwttoken;
                AppSession.UserName = result.UserName;

                lblStatus.Text = "✅ Web login başarılı. Token alındı!"+ result.Jwttoken ;
            }
            catch (Exception ex)
            {
                lblStatus.Text = $"⚠️ Hata: {ex.Message}";
            }
        }

        // Hub’a bağlanma
        private async void btnConnectHub_Click(object sender, EventArgs e)
{
    try
    {
        if (string.IsNullOrEmpty(AppSession.JwtToken))
        {
            lblStatus.Text = "⚠️ Önce Web login yapıp token almalısın.";
            return;
        }

        lblStatus.Text = "🔄 Hub’a bağlanıyor...";

        await HubConnectionManager.ConnectAsync(AppSession.JwtToken, txtHubUrl.Text.Trim());

        HubConnectionManager.OrderHub.On<string>("ReceiveMessage", (json) =>
        {
            var hubMessage = Newtonsoft.Json.JsonConvert.DeserializeObject<HubMessage>(json);
            if (hubMessage == null) return;

            HubMessage response = null;

            if (hubMessage.MessageType == "REQ")
            {
                // Hangisi geldiğine göre yönlendir
                switch (hubMessage.MessageRequestCode)
                {
                    case "RESTAURANTMENU":
                    case "WEBORDER":
                        response = OrderService.HandleRequest(hubMessage);
                        break;

                    case "GUNSONUOZET":
                    case "SATISLAR":
                    case "ACIKSATISLAR":
                    case "ONLINESIPARISOZET":
                        response = BossService.HandleRequest(hubMessage);
                        break;

                    default:
                        response = new HubMessage
                        {
                            MessageFromUser = hubMessage.MessageToUser,
                            MessageToUser = hubMessage.MessageFromUser,
                            MessageType = "ERR",
                            MessageRequestCode = hubMessage.MessageRequestCode,
                            MessageSubject = "Bilinmeyen Request",
                            MessageBody = $"Desteklenmeyen kod: {hubMessage.MessageRequestCode}"
                        };
                        break;

                }


                if (response != null)
                {
                    HubConnectionManager.OrderHub.SendAsync("SendOrderMessage",
                        response.MessageToUser, response.ToJson());
                }
            }

            this.Invoke(new Action(() =>
            {
                lblStatus.Text = $"📩 {hubMessage.MessageType} - {hubMessage.MessageRequestCode}";
            }));
        });

        lblStatus.Text = "✅ Hub bağlantısı başarılı!";
    }
    catch (Exception ex)
    {
        lblStatus.Text = $"⚠️ Hub Hatası: {ex.Message}";
    }
}

        
        
    }
}
