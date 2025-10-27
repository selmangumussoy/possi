using Microsoft.Data.SqlClient; 

namespace WinFormsApp1.Service
{
    public static class DatabaseService
    {
        public static string ConnectionString { get; private set; }

        public static bool Connect(string server, string dbName, string user, string password)
        {
            try
            {
                // SQL Server için:
                ConnectionString = $"Server={server};Database={dbName};User Id={user};Password={password};TrustServerCertificate=True;";

                using (var conn = new SqlConnection(ConnectionString))
                {
                    conn.Open();
                    return true; // Bağlantı başarılı
                }
            }
            catch
            {
                return false;
            }
        }
    }
}