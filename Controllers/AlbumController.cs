﻿using BandAPI.Models;
using BandAPI.Services;
using BandAPI.Helpers;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;

namespace BandAPI.Controllers
{
    [ApiController]
    [Route("api/bands/{bandId}/album")]
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

        [HttpGet(Name = "GetAlbumsForBand")]
        [HttpHead]
        public ActionResult<IEnumerable<AlbumDto>> GetAlbumsForBand(Guid bandId)
        {
            if (!_bandAlbumRepository.BandExists(bandId))
                return NotFound();

            var albums = _bandAlbumRepository.GetAlbums(bandId);

            return Ok(_mapper.Map<IEnumerable<AlbumDto>>(albums));
        }

        [HttpGet("{albumId}", Name = "GetAlbumForBand")]
        [ResponseCache(Duration =120)]
        public ActionResult<AlbumDto> GetAlbumForBand(Guid bandId, Guid albumId)
        {
            if (!_bandAlbumRepository.BandExists(bandId))
                return NotFound();

            var album = _bandAlbumRepository.GetAlbum(bandId, albumId);

            if (album == null)
                return NotFound();

            return Ok(_mapper.Map<AlbumDto>(album));
        }


        [HttpPost(Name = "CreateAlbumForBand")]
        public ActionResult<AlbumDto> CreateAlbumForBand(Guid bandId, [FromBody] AlbumForCreatingDto albumDto)
        {
            if (!_bandAlbumRepository.BandExists(bandId))
                return NotFound();

            var album = _mapper.Map<Entities.Album>(albumDto);
            _bandAlbumRepository.AddAlbum(bandId, album);
            _bandAlbumRepository.Save();

            var albumToReturn = _mapper.Map<AlbumDto>(album);

            return CreatedAtRoute("GetAlbumForBand", new { bandId = bandId, albumId = albumToReturn.Id }, albumToReturn);
        }


        [HttpPut("{albumId}")]
        public ActionResult UpdateAlbumForBand(Guid bandId, Guid albumId, [FromBody] AlbumForUpdatingDto albumDto)
        {
            if (!_bandAlbumRepository.BandExists(bandId))
                return NotFound();

            var album = _bandAlbumRepository.GetAlbum(bandId, albumId);

            if (album == null)
            {
                var albumToAdd = _mapper.Map<Entities.Album>(albumDto);
                albumToAdd.Id = albumId;
                _bandAlbumRepository.AddAlbum(bandId, albumToAdd);
                _bandAlbumRepository.Save();

                var albumToReturn = _mapper.Map<AlbumDto>(albumToAdd);

                return CreatedAtRoute("GetAlbumForBand", new { bandId = bandId, albumId = albumToReturn.Id }, albumToReturn);
            }

            _mapper.Map(albumDto, album);
            _bandAlbumRepository.UpdateAlbum(album);
            _bandAlbumRepository.Save();

            return NoContent();
        }

        [HttpPatch("{albumId}")]
        public ActionResult PartiallyUpdateAlbumForBand(Guid bandId, Guid albumId, [FromBody] JsonPatchDocument<AlbumForUpdatingDto> patchDocument)
        {
            if (!_bandAlbumRepository.BandExists(bandId))
                return NotFound();

            var album = _bandAlbumRepository.GetAlbum(bandId, albumId);

            if (album == null)
            {
                var albumDto = new AlbumForUpdatingDto();
                patchDocument.ApplyTo(albumDto);
                var albumToAdd = _mapper.Map<Entities.Album>(albumDto);
                albumToAdd.Id = albumId;
                _bandAlbumRepository.AddAlbum(bandId, albumToAdd);
                _bandAlbumRepository.Save();

                var albumToReturn = _mapper.Map<AlbumDto>(albumToAdd);

                return CreatedAtRoute("GetAlbumForBand", new { bandId = bandId, albumId = albumToReturn.Id }, albumToReturn);

            }

            var albumToPatch = _mapper.Map<AlbumForUpdatingDto>(album);
            patchDocument.ApplyTo(albumToPatch, ModelState);

            if (!TryValidateModel(albumToPatch))
                return ValidationProblem(ModelState);

            _mapper.Map(albumToPatch, album);
            _bandAlbumRepository.UpdateAlbum(album);
            _bandAlbumRepository.Save();

            return NoContent();
        }

        [HttpDelete("{albumId}")]
        public ActionResult DeleteAlbumForBand(Guid bandId, Guid albumId)
        {
            if (!_bandAlbumRepository.BandExists(bandId))
                return NotFound();

            var album = _bandAlbumRepository.GetAlbum(bandId, albumId);

            if (album == null)
                return NotFound();

            _bandAlbumRepository.DeleteAlbum(album);
            _bandAlbumRepository.Save();

            return NoContent();
        }
    }
}
