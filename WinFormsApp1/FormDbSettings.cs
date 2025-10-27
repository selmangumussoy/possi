
using Microsoft.Data.SqlClient;
using WinFormsApp1.Models;
using WinFormsApp1.Service;

namespace WinFormsApp1
{
    public partial class FormDbSettings : Form
    {
        public FormDbSettings()
        {
            InitializeComponent();
            LoadSettings();
        }

        private void LoadSettings()
        {
            var settings = ConfigService.LoadDbSettings();
            txtServer.Text = settings.Server;
            txtDatabase.Text = settings.Database;
            txtUser.Text = settings.User;
            txtPassword.Text = settings.Password;
            chkWindowsAuth.Checked = settings.UseWindowsAuth;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                var settings = new DbSettings
                {
                    Server = txtServer.Text,
                    Database = txtDatabase.Text,
                    User = txtUser.Text,
                    Password = txtPassword.Text,
                    UseWindowsAuth = chkWindowsAuth.Checked
                };

                ConfigService.SaveDbSettings(settings);

                MessageBox.Show("✅ Ayarlar kaydedildi.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ Hata: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnTest_Click(object sender, EventArgs e)
        {
            try
            {
                string connStr = chkWindowsAuth.Checked
                    ? $"Server={txtServer.Text};Database={txtDatabase.Text};Integrated Security=True;TrustServerCertificate=True;Encrypt=False;"
                    : $"Server={txtServer.Text};Database={txtDatabase.Text};User Id={txtUser.Text};Password={txtPassword.Text};TrustServerCertificate=True;Encrypt=False;";

                using var conn = new SqlConnection(connStr);
                conn.Open();

                MessageBox.Show("✅ Bağlantı başarılı!", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ Hata: {ex.Message}", "Bağlantı Hatası", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
