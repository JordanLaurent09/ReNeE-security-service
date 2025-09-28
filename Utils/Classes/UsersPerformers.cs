namespace security_service.Utils.Classes
{
    public class UsersPerformers
    {
        
        public int Id { get; set; }
    
        public int UserId { get; set; }
     
        public int PerformerId { get; set; }

        public UsersPerformers() { }

        public UsersPerformers(int userId, int performerId)
        {
            UserId = userId;
            PerformerId = performerId;
        }
    }
}
