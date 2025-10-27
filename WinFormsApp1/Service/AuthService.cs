using System.Text;
using Newtonsoft.Json;
using WinFormsApp1.Models;

namespace WinFormsApp1.Service
{
    public class AuthService
    {
        private readonly string _baseUrl;

        public AuthService(string baseUrl)
        {
            _baseUrl = baseUrl.TrimEnd('/');
        }

        public async Task<LoginResponse> LoginAsync(string email, string password)
        {
            using var client = new HttpClient();

            var login = new { email, password };

            var json = JsonConvert.SerializeObject(login);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            // ✅ Login endpoint sabit
            var response = await client.PostAsync($"{_baseUrl}/api/auth/login", content);

            if (!response.IsSuccessStatusCode)
                return null;

            var responseBody = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<LoginResponse>(responseBody);
        }
    }
}