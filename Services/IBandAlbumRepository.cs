using BandAPI.Entities;
using BandAPI.Helpers;
using System;
using System.Collections.Generic;

namespace BandAPI.Services
{
    public interface IBandAlbumRepository
    {
        IEnumerable<Album> GetAlbums(Guid bandId);
        Album GetAlbum(Guid bandId, Guid albumId);
        void AddAlbum(Guid bandId, Album album);
        void UpdateAlbum(Album album);
        void DeleteAlbum(Album album);

        IEnumerable<Band> GetBands();
        IEnumerable<Band> GetBands(IEnumerable<Guid> bandsIds);
        IEnumerable<Band> GetBands(BandResourceParameters bandResourceParameters);
        Band GetBand(Guid bandId);
        void AddBand(Band band);
        void UpdateBand(Band band);
        void DeleteBand(Band band);

        bool BandExists(Guid bandId);
        bool AlbumExists(Guid albumId);
        bool Save();
    }
}
