using BandAPI.Entities;
using Microsoft.EntityFrameworkCore;

namespace BandAPI.DbContexts
{
    public class BandAlbumContext : DbContext
    {
        public BandAlbumContext(DbContextOptions<BandAlbumContext> options) : base(options)
        {
        }

        public DbSet<Band> Bands { get; set; }
        public DbSet<Album> Albums { get; set; }
    }
}
