namespace MvcMusicStore.Models
{
    using System.Data.Entity;

    public class MusicStoreEntities : DbContext
    {
        public DbSet<Album> Albums { get; set; }

        public DbSet<Genre> Genres { get; set; }

        public DbSet<Artist> Artists { get; set; }

        public DbSet<ActionLog> ActionLog { get; set; }
    }
}
