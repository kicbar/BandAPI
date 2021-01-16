using BandAPI.Data;
using BandAPI.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BandAPI.Services
{
    public class BandAlbumRepository : IBandAlbumRepository
    {
        private readonly BandAlbumContext _bandAlbumContext;

        public BandAlbumRepository(BandAlbumContext bandAlbumContext)
        {
            _bandAlbumContext = bandAlbumContext ?? throw new ArgumentNullException(nameof(bandAlbumContext));
        }

        public void AddAlbum(Guid bandId, Album album)
        {
            if (bandId == Guid.Empty)
                throw new ArgumentNullException(nameof(bandId));

            if (album == null)
                throw new ArgumentNullException(nameof(album));

            album.BandId = bandId;

            _bandAlbumContext.Albums.Add(album);
        }

        public void AddBand(Guid bandId, Band band)
        {
            if (band == null)
                throw new ArgumentNullException(nameof(band));

            _bandAlbumContext.Bands.Add(band);
        }

        public bool AlbumExists(Guid albumId)
        {
            if (albumId == Guid.Empty)
                throw new ArgumentNullException(nameof(albumId));

            return _bandAlbumContext.Albums.Any(a => a.Id == albumId);
        }

        public bool BandExists(Guid bandId)
        {
            if (bandId == Guid.Empty)
                throw new ArgumentNullException(nameof(bandId));

            return _bandAlbumContext.Bands.Any(a => a.Id == bandId);
        }

        public void DeleteAlbum(Album album)
        {
            if (album == null)
                throw new ArgumentNullException(nameof(album));

            _bandAlbumContext.Albums.Remove(album);
        }

        public void DeleteBand(Band band)
        {
            if (band == null)
                throw new ArgumentNullException(nameof(band));

            _bandAlbumContext.Bands.Remove(band);
        }

        public Album GetAlbum(Guid bandId, Guid albumId)
        {
            if (bandId == Guid.Empty)
                throw new ArgumentNullException(nameof(bandId));

            if (albumId == null)
                throw new ArgumentNullException(nameof(albumId));

            return _bandAlbumContext.Albums.Where(a => a.BandId == bandId && a.Id == albumId)
                .FirstOrDefault();
        }

        public IEnumerable<Album> GetAlbums(Guid bandId)
        {
            if (bandId == Guid.Empty)
                throw new ArgumentNullException(nameof(bandId));

            return _bandAlbumContext.Albums.Where(a => a.BandId == bandId)
                .OrderBy(a => a.Title)
                .ToList();
        }

        public Band GetBand(Guid bandId)
        {
            if (bandId == Guid.Empty)
                throw new ArgumentNullException(nameof(bandId));

            return _bandAlbumContext.Bands.FirstOrDefault(b => b.Id == bandId);
        }

        public IEnumerable<Band> GetBands()
        {
            return _bandAlbumContext.Bands.ToList();
        }

        public IEnumerable<Band> GetBands(IEnumerable<Guid> bandsIds)
        {
            if (bandsIds == null)
                throw new ArgumentNullException(nameof(bandsIds));

            return _bandAlbumContext.Bands.Where(b => bandsIds.Contains(b.Id))
                .OrderBy(b => b.Name)
                .ToList();
        }

        public IEnumerable<Band> GetBands(string mainGenre, string searchQuery)
        {
            if (string.IsNullOrWhiteSpace(mainGenre) && string.IsNullOrWhiteSpace(searchQuery))
                return GetBands();

            var collection = _bandAlbumContext.Bands as IQueryable<Band>;

            if (!string.IsNullOrWhiteSpace(mainGenre))
            {
                mainGenre = mainGenre.Trim();
                collection = collection.Where(b => b.MainGenre == mainGenre);
            }
            if (!string.IsNullOrWhiteSpace(searchQuery))
            {
                searchQuery = searchQuery.Trim();
                collection = collection.Where(b => b.Name.Contains(searchQuery));
            }

            return collection.ToList();
        }

        public bool Save()
        {
            return (_bandAlbumContext.SaveChanges() >= 0);
        }

        public void UpdateAlbum(Guid bandId, Album album)
        {
            //not implemented
        }

        public void UpdateBand(Guid bandId, Band band)
        {
            //not implemented
        }
    }
}
