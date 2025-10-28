using Microsoft.AspNetCore.SignalR.Client;
using WinFormsApp1.Service;

namespace WinFormsApp1
{
    public partial class Form1 : Form
    {
        private System.Windows.Forms.Timer tmrBoss;

        public Form1()
        {
            InitializeComponent();

            // 🔄 BossHub bağlantı kontrolü için timer
            tmrBoss = new System.Windows.Forms.Timer();
            tmrBoss.Interval = 10000; // her 10 saniyede bir kontrol et
            tmrBoss.Tick += tmrBoss_Tick;

            // ✅ DB bağlantı string'ini başlat
            LoadDbConnection();
        }

        private void LoadDbConnection()
        {
            try
            {
                var dbSettings = ConfigService.LoadDbSettings();
                AppSession.DbConnectionString = dbSettings.BuildConnectionString();
                
                if (!string.IsNullOrEmpty(AppSession.DbConnectionString))
                {
                    lblStatus.Text = "✅ DB ayarları yüklendi";
                }
            }
            catch
            {
                lblStatus.Text = "⚠️ DB ayarları bulunamadı - Ayarlar butonuna tıklayın";
            }
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

                lblStatus.Text = "✅ Web login başarılı. Token alındı!" + result.Jwttoken;
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

                // TextBox'tan URL alınıyor
                string hubUrl = txtHubUrl.Text.Trim();

                await HubConnectionManager.ConnectAsync(AppSession.JwtToken, hubUrl);

                    HubConnectionManager.MessageReceived += async (toUser, json) =>
                    {
                        this.Invoke(new Action(() =>
                        {
                            lblStatus.Text = $"📥 Mesaj alındı: {json.Substring(0, Math.Min(100, json.Length))}...";
                        }));

                        var hubMessage = Newtonsoft.Json.JsonConvert.DeserializeObject<HubMessage>(json);
                        if (hubMessage == null)
                        {
                            this.Invoke(new Action(() =>
                            {
                                lblStatus.Text = "❌ Mesaj parse edilemedi";
                            }));
                            return;
                        }

                        HubMessage response = null;

                    if (hubMessage.MessageType == "REQ")
                    {
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
                            // ✅ Debug: Gönderilecek mesajı logla
                            var responseJson = response.ToJson();
                            this.Invoke(new Action(() =>
                            {
                                lblStatus.Text = $"📤 Cevap gönderiliyor...";
                            }));

                            await HubConnectionManager.HubConnection.SendAsync(
                                "SendBossMessage",
                                response.MessageToUser,
                                responseJson
                            );

                            this.Invoke(new Action(() =>
                            {
                                lblStatus.Text = $"✅ Cevap gönderildi!";
                            }));
                        }
                }

                this.Invoke(new Action(() =>
                {
                    lblStatus.Text = $"📩 {hubMessage.MessageType} - {hubMessage.MessageRequestCode}";
                }));
            };

            lblStatus.Text = "✅ Hub bağlantısı başarılı!";
            tmrBoss.Start();
        }
        catch (Exception ex)
        {
            lblStatus.Text = $"⚠️ Hub Hatası: {ex.Message}";
        }
}

        // DB bağlantısı
        private void btnDbSettings_Click(object sender, EventArgs e)
        {
            using (var dbSettingsForm = new FormDbSettings())
            {
                if (dbSettingsForm.ShowDialog() == DialogResult.OK)
                {
                    var dbSettings = ConfigService.LoadDbSettings();
                    AppSession.DbConnectionString = dbSettings.BuildConnectionString();
                }
            }
        }

        // 🔄 Timer tick - bağlantı kontrolü
        private async void tmrBoss_Tick(object sender, EventArgs e)
        {
            try
            {
                if (HubConnectionManager.HubConnection == null ||
                    HubConnectionManager.HubConnection.State != HubConnectionState.Connected)
                {
                    lblStatus.Text = "🔄 BossHub yeniden bağlanıyor...";
                    await HubConnectionManager.ConnectAsync(AppSession.JwtToken, txtHubUrl.Text.Trim());
                    lblStatus.Text = "✅ BossHub tekrar bağlandı!";
                }
            }
            catch (Exception ex)
            {
                lblStatus.Text = $"⚠️ Reconnect hatası: {ex.Message}";
            }
        }
    }
}
