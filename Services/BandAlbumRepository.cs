using BandAPI.Data;
using BandAPI.Entities;
using BandAPI.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BandAPI.Services
{
    public class BandAlbumRepository : IBandAlbumRepository
    {
        private readonly BandAlbumContext _bandAlbumContext;
        private readonly IPropertyMappingService _propertyMappingService;

        public BandAlbumRepository(BandAlbumContext bandAlbumContext, IPropertyMappingService propertyMappingService)
        {
            _bandAlbumContext = bandAlbumContext ?? throw new ArgumentNullException(nameof(bandAlbumContext));
            _propertyMappingService = propertyMappingService ?? throw new ArgumentNullException(nameof(bandAlbumContext));
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

        public void AddBand(Band band)
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

        public PagedList<Band> GetBands(BandResourceParameters bandResourceParameters)
        {
            if (bandResourceParameters == null)
                throw new ArgumentNullException(nameof(bandResourceParameters));

            var collection = _bandAlbumContext.Bands as IQueryable<Band>;

            if (!string.IsNullOrWhiteSpace(bandResourceParameters.MainGenre))
            {
                var mainGenre = bandResourceParameters.MainGenre.Trim();
                collection = collection.Where(b => b.MainGenre == mainGenre);
            }
            if (!string.IsNullOrWhiteSpace(bandResourceParameters.SearchQuery))
            {
                var searchQuery = bandResourceParameters.SearchQuery.Trim();
                collection = collection.Where(b => b.Name.Contains(searchQuery));
            }

            if (!string.IsNullOrWhiteSpace(bandResourceParameters.OrderBy))
            { 
                var bandPropertyMappingDictionary = _propertyMappingService.GetPropertyMapping<Models.BandDto, Entities.Band>();

                collection = collection.ApplySort(bandResourceParameters.OrderBy,
                    bandPropertyMappingDictionary);
            }

            return PagedList<Band>.Create(collection, bandResourceParameters.PageNumber, bandResourceParameters.PageSize);
        }

        public bool Save()
        {
            return (_bandAlbumContext.SaveChanges() >= 0);
        }

        public void UpdateAlbum(Album album)
        {
            //not implemented
        }

        public void UpdateBand(Band band)
        {
            //not implemented
        }
    }
}
