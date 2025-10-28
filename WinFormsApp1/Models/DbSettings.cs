
namespace WinFormsApp1.Models
{
    public class DbSettings
    {
        public string Server { get; set; } = "";
        public string Database { get; set; } = "";
        public string User { get; set; } = "";
        public string Password { get; set; } = "";
        public bool UseWindowsAuth { get; set; } = true;
        
        public string BuildConnectionString()
        {
            if (UseWindowsAuth)
                return $"Server={Server};Database={Database};Integrated Security=True;TrustServerCertificate=True;Encrypt=False;";
            else
                return $"Server={Server};Database={Database};User Id={User};Password={Password};TrustServerCertificate=True;Encrypt=False;";
        }
    }
}
