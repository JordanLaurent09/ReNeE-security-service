using System.ComponentModel.DataAnnotations.Schema;

namespace security_service.Utils.Classes
{
    public class UsersAlbums
    {
        [Column("id")]
        public int Id { get; set; }

        [Column("albumid")]
        public int AlbumId { get; set; }

        [Column("userid")]
        public int UserId { get; set; }

        [Column("performerid")]
        public int PerformerId { get; set; }

        public UsersAlbums() { }

        public UsersAlbums(int albumId, int userId, int performerId)
        {
            AlbumId = albumId;
            UserId = userId;
            PerformerId = performerId;
        }
    }
}
