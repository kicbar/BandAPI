using BandAPI.Models;
using BandAPI.Services;
using BandAPI.Helpers;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;

namespace BandAPI.Controllers
{
    [ApiController]
    [Route("api/band/{bandId}/album")]
    public class AlbumController : ControllerBase
    {
        private readonly IBandAlbumRepository _bandAlbumRepository;
        private readonly IMapper _mapper;

        public AlbumController(IBandAlbumRepository bandAlbumRepository, IMapper mapper)
        {
            _bandAlbumRepository = bandAlbumRepository ??
                throw new ArgumentNullException(nameof(bandAlbumRepository));
            _mapper = mapper ??
                throw new ArgumentNullException(nameof(mapper));
        }

        [HttpGet]
        [HttpHead]
        public ActionResult<IEnumerable<AlbumDto>> GetAlbumsForBand(Guid bandId)
        {
            if (!_bandAlbumRepository.BandExists(bandId))
                return NotFound();

            var albums = _bandAlbumRepository.GetAlbums(bandId);

            return Ok(_mapper.Map<IEnumerable<AlbumDto>>(albums));
        }

        [HttpGet("{albumId}", Name = "GetAlbumForBand")]
        [HttpHead]
        public ActionResult<AlbumDto> GetAlbumForBand(Guid bandId, Guid albumId)
        {
            if (!_bandAlbumRepository.BandExists(bandId))
                return NotFound();

            var album = _bandAlbumRepository.GetAlbum(bandId, albumId);

            if (album == null)
                return NotFound();

            return Ok(_mapper.Map<AlbumDto>(album));
        }


        [HttpPost]
        public ActionResult<AlbumDto> CreateAlbumForBand(Guid bandId, [FromBody] AlbumForCreatingDto albumDto)
        {
            if (!_bandAlbumRepository.BandExists(bandId))
                return NotFound();

            var album = _mapper.Map<Entities.Album>(albumDto);
            _bandAlbumRepository.AddAlbum(bandId, album);
            _bandAlbumRepository.Save();

            var albumToReturn = _mapper.Map<AlbumDto>(album);

            return CreatedAtRoute("GetAlbumForBand", new {bandId = bandId, albumId =  albumToReturn.Id}, albumToReturn);
        }
    }
}
