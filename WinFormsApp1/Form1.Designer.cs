namespace WinFormsApp1
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;
        private TextBox txtBaseUrl;
        private TextBox txtWebEmail;
        private TextBox txtWebPassword;
        private TextBox txtHubUrl;
        private Button btnLogin;
        private Button btnConnectHub;
        private Label lblStatus;

        // ✅ DB için sadece ayar butonu
        private Button btnDbSettings;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.txtBaseUrl = new TextBox();
            this.txtWebEmail = new TextBox();
            this.txtWebPassword = new TextBox();
            this.txtHubUrl = new TextBox();
            this.btnLogin = new Button();
            this.btnConnectHub = new Button();
            this.lblStatus = new Label();
            this.btnDbSettings = new Button();

            this.SuspendLayout();
            // 
            // txtBaseUrl
            // 
            this.txtBaseUrl.Location = new Point(20, 20);
            this.txtBaseUrl.Size = new Size(300, 23);
            this.txtBaseUrl.PlaceholderText = "Base URL (örn: https://boss.possi.com.tr)";
            // 
            // txtWebEmail
            // 
            this.txtWebEmail.Location = new Point(20, 60);
            this.txtWebEmail.Size = new Size(300, 23);
            this.txtWebEmail.PlaceholderText = "Web Email";
            // 
            // txtWebPassword
            // 
            this.txtWebPassword.Location = new Point(20, 100);
            this.txtWebPassword.Size = new Size(300, 23);
            this.txtWebPassword.PasswordChar = '*';
            this.txtWebPassword.PlaceholderText = "Web Şifre";
            // 
            // txtHubUrl
            // 
            this.txtHubUrl.Location = new Point(20, 140);
            this.txtHubUrl.Size = new Size(300, 23);
            this.txtHubUrl.PlaceholderText = "Hub URL (örn: https://boss.possi.com.tr/orderhub)";
            // 
            // btnLogin
            // 
            this.btnLogin.Location = new Point(20, 180);
            this.btnLogin.Size = new Size(140, 30);
            this.btnLogin.Text = "🔑 Web Login";
            this.btnLogin.Click += new EventHandler(this.btnLogin_Click);
            // 
            // btnConnectHub
            // 
            this.btnConnectHub.Location = new Point(180, 180);
            this.btnConnectHub.Size = new Size(140, 30);
            this.btnConnectHub.Text = "📡 Hub’a Bağlan";
            this.btnConnectHub.Click += new EventHandler(this.btnConnectHub_Click);
            // 
            // lblStatus
            // 
            this.lblStatus.Location = new Point(20, 220);
            this.lblStatus.Size = new Size(300, 50);
            this.lblStatus.Text = "Durum: Bekleniyor...";
            // 
            // btnDbSettings
            // 
            this.btnDbSettings.Location = new Point(20, 280);
            this.btnDbSettings.Size = new Size(300, 30);
            this.btnDbSettings.Text = "⚙️ DB Ayarları";
            this.btnDbSettings.Click += new EventHandler(this.btnDbSettings_Click);
            // 
            // Form1
            // 
            this.ClientSize = new Size(360, 350);
            this.Controls.Add(this.txtBaseUrl);
            this.Controls.Add(this.txtWebEmail);
            this.Controls.Add(this.txtWebPassword);
            this.Controls.Add(this.txtHubUrl);
            this.Controls.Add(this.btnLogin);
            this.Controls.Add(this.btnConnectHub);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.btnDbSettings);

            this.Text = "OrderHub Client";
            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}
