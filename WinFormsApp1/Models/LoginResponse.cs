namespace WinFormsApp1.Models
{
    public class LoginResponse
    {
        public int UserId { get; set; }
        public int CompanyId { get; set; }
        public int UserTypeId { get; set; }
        public string UserEmail { get; set; }
        public string UserName { get; set; }
        public string UserFullName { get; set; }
        public bool Success { get; set; }
        public string Message { get; set; }
        public string Jwttoken { get; set; }
    }
}