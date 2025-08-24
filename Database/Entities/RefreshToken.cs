using security_service.Utils.Classes;

namespace security_service.Database.Entities
{
    public class RefreshToken
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public string Username { get; set; }

        public string Role { get; set; }

        public string? TokenValue { get; set; }

        public DateTime? ExpiryDate { get; set; }
    }
}
