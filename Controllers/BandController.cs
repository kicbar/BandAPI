﻿using BandAPI.Models;
using BandAPI.Services;
using BandAPI.Helpers;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using System.Text.Json;
using BandAPI.Entities;

namespace BandAPI.Controllers
{
    [ApiController]
    [Route("api/bands")]
    public class BandController : ControllerBase
    {
        private readonly IBandAlbumRepository _bandAlbumRepository;
        private readonly IMapper _mapper;
        private readonly IPropertyMappingService _propertyMappingService;

        public BandController(IBandAlbumRepository bandAlbumRepository, IMapper mapper, IPropertyMappingService propertyMappingService)
        {
            _bandAlbumRepository = bandAlbumRepository ??
                throw new ArgumentNullException(nameof(bandAlbumRepository));
            _mapper = mapper ??
                throw new ArgumentNullException(nameof(mapper));
            _propertyMappingService = propertyMappingService ??
                throw new ArgumentNullException(nameof(propertyMappingService));
        }

        [HttpGet(Name = "GetBands")]
        [HttpHead]
        public ActionResult<IEnumerable<BandDto>> GetBands([FromQuery] BandResourceParameters bandResourceParameters)
        {
            if (!_propertyMappingService.ValidMappingExist<BandDto, Band>(bandResourceParameters.OrderBy))
                return BadRequest();

            var bands = _bandAlbumRepository.GetBands(bandResourceParameters);

            var previousPageLink = bands.HasPrevious ? CreateBandsUri(bandResourceParameters, UriType.PreviousPage) : null;
            var previousNextLink = bands.HasNext ? CreateBandsUri(bandResourceParameters, UriType.NextPage) : null;

            var metaData = new
            {
                totalCount = bands.TotalCount,
                pageSize = bands.PageSize,
                currentPage = bands.CurrentPage,
                totalPages = bands.TotalPages,
                previousPageLink = previousPageLink,
                previousNextLink = previousNextLink
            };

            Response.Headers.Add("Pagination", JsonSerializer.Serialize(metaData));

            return Ok(_mapper.Map<IEnumerable<BandDto>>(bands));
        }

        [HttpGet("{bandId}", Name = "GetBand")]
        public IActionResult GetBand(Guid bandId)
        {
            var band = _bandAlbumRepository.GetBand(bandId);

            if (band == null)
                return NotFound();

            return Ok(band);
        }

        [HttpPost]
        public ActionResult<BandDto> CreateBand([FromBody] BandForCreatingDto bandDto)
        {
            var band = _mapper.Map<Entities.Band>(bandDto);
            _bandAlbumRepository.AddBand(band);
            _bandAlbumRepository.Save();

            var bandToReturn = _mapper.Map<BandDto>(band);

            return CreatedAtRoute("GetBand", new { bandId = bandToReturn.Id}, bandToReturn);
        }

        [HttpDelete("{bandId}")]
        public ActionResult DeleteBand(Guid bandId)
        {
            var band = _bandAlbumRepository.GetBand(bandId);
            if (band == null)
                return NotFound();
            
            _bandAlbumRepository.DeleteBand(band);
            _bandAlbumRepository.Save();

            return NoContent();
        }

        private string CreateBandsUri(BandResourceParameters bandResourceParameters, UriType uriType)
        {

            switch (uriType)
            {
                case UriType.PreviousPage:
                    return Url.Link("GetBands", new 
                    {
                        orderBy = bandResourceParameters.OrderBy,
                        pageNumber = bandResourceParameters.PageNumber - 1,
                        pageSize = bandResourceParameters.PageSize,
                        mainGenre = bandResourceParameters.MainGenre,
                        searchQuery = bandResourceParameters.SearchQuery
                    });

                case UriType.NextPage:
                    return Url.Link("GetBands", new
                    {
                        orderBy = bandResourceParameters.OrderBy,
                        pageNumber = bandResourceParameters.PageNumber + 1,
                        pageSize = bandResourceParameters.PageSize,
                        mainGenre = bandResourceParameters.MainGenre,
                        searchQuery = bandResourceParameters.SearchQuery
                    });
                default:
                    return Url.Link("GetBands", new
                    {
                        orderBy = bandResourceParameters.OrderBy,
                        pageNumber = bandResourceParameters.PageNumber,
                        pageSize = bandResourceParameters.PageSize,
                        mainGenre = bandResourceParameters.MainGenre,
                        searchQuery = bandResourceParameters.SearchQuery
                    });
            }
        }
    }
}
