
namespace WinFormsApp1.Models
{
    public class DbSettings
    {
        public string Server { get; set; } = "";
        public string Database { get; set; } = "";
        public string User { get; set; } = "";
        public string Password { get; set; } = "";
        public bool UseWindowsAuth { get; set; } = true;
    }
}
