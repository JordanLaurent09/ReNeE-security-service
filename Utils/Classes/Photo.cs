using System.ComponentModel.DataAnnotations.Schema;

namespace security_service.Utils.Classes
{
    public class Photo
    {
        public int Id { get; set; }
       
        public int PerformerId { get; set; }
    
        public User? User { get; set; }      
     
        public int UserId { get; set; }

        public string? Image { get; set; }


        public Photo() { }

        public Photo(int performerId, int userId, string image)
        {
            PerformerId = performerId;
            UserId = userId;
            Image = image;
        }
    }
}
