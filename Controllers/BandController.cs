using AutoMapper;
using BandAPI.Entities;
using BandAPI.Helpers;
using BandAPI.Models;
using BandAPI.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace BandAPI.Controllers
{
    [ApiController]
    [Route("api/bands")]
    public class BandController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IBandAlbumRepository _bandAlbumRepository;
        private readonly IPropertyMappingService _propertyMappingService;
        private readonly IPropertyValidationService _propertyValidationService;

        public BandController(IBandAlbumRepository bandAlbumRepository, IMapper mapper, IPropertyMappingService propertyMappingService, IPropertyValidationService propertyValidationService)
        {
            _bandAlbumRepository = bandAlbumRepository ??
                throw new ArgumentNullException(nameof(bandAlbumRepository));
            _mapper = mapper ??
                throw new ArgumentNullException(nameof(mapper));
            _propertyMappingService = propertyMappingService ??
                throw new ArgumentNullException(nameof(propertyMappingService));
            _propertyValidationService = propertyValidationService ??
                throw new ArgumentNullException(nameof(propertyValidationService));
        }

        [HttpGet(Name = "GetBands")]
        [ResponseCache(Duration = 120)]
        public IActionResult GetBands([FromQuery] BandResourceParameters bandResourceParameters)
        {
            if (!_propertyMappingService.ValidMappingExist<BandDto, Band>(bandResourceParameters.OrderBy))
                return BadRequest();

            if (!_propertyValidationService.HasValidProperty<BandDto>(bandResourceParameters.Fields))
                return BadRequest();

            var bands = _bandAlbumRepository.GetBands(bandResourceParameters);

            var metaData = new
            {
                totalCount = bands.TotalCount,
                pageSize = bands.PageSize,
                currentPage = bands.CurrentPage,
                totalPages = bands.TotalPages
            };

            Response.Headers.Add("Pagination", JsonSerializer.Serialize(metaData));

            var links = CreateLinksForBands(bandResourceParameters, bands.HasNext, bands.HasPrevious);
            var shapedBands = _mapper.Map<IEnumerable<BandDto>>(bands).ShapeData(bandResourceParameters.Fields);
            var shapedBandsWithLinks = shapedBands.Select(band =>
            {
                var bandAsDictionary = band as IDictionary<string, object>;
                var bandLinks = CreateLinksForBand((Guid)bandAsDictionary["Id"], null);
                bandAsDictionary.Add("links", bandLinks);
                return bandAsDictionary;
            });

            var linkedCollectionResourcce = new
            {
                value = shapedBandsWithLinks,
                links
            };

            return Ok(linkedCollectionResourcce);
        }

        [HttpGet("{bandId}", Name = "GetBand")]
        public IActionResult GetBand(Guid bandId, string fields)
        {
            if (!_propertyValidationService.HasValidProperty<BandDto>(fields))
                return BadRequest();

            var band = _bandAlbumRepository.GetBand(bandId);

            if (band == null)
                return NotFound();

            var links = CreateLinksForBand(bandId, fields);
            var linkedResourceToReturn = _mapper.Map<BandDto>(band).ShapeDate(fields) as IDictionary<string, object>;
            linkedResourceToReturn.Add("links", links);

            return Ok(linkedResourceToReturn);
        }

        [HttpPost]
        public ActionResult<BandDto> CreateBand([FromBody] BandForCreatingDto bandDto)
        {
            var band = _mapper.Map<Entities.Band>(bandDto);
            _bandAlbumRepository.AddBand(band);
            _bandAlbumRepository.Save();

            var bandToReturn = _mapper.Map<BandDto>(band);

            var links = CreateLinksForBand(bandToReturn.Id, null);
            var linkedResourceToReturn = bandToReturn.ShapeDate(null) as IDictionary<string, object>;
            linkedResourceToReturn.Add("links", links);

            return CreatedAtRoute("GetBand", new { bandId = linkedResourceToReturn["Id"] }, linkedResourceToReturn);
        }

        [HttpDelete("{bandId}", Name = "DeleteBand")]
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
                        fields = bandResourceParameters.Fields,
                        orderBy = bandResourceParameters.OrderBy,
                        pageNumber = bandResourceParameters.PageNumber - 1,
                        pageSize = bandResourceParameters.PageSize,
                        mainGenre = bandResourceParameters.MainGenre,
                        searchQuery = bandResourceParameters.SearchQuery
                    });
                case UriType.NextPage:
                    return Url.Link("GetBands", new
                    {
                        fields = bandResourceParameters.Fields,
                        orderBy = bandResourceParameters.OrderBy,
                        pageNumber = bandResourceParameters.PageNumber + 1,
                        pageSize = bandResourceParameters.PageSize,
                        mainGenre = bandResourceParameters.MainGenre,
                        searchQuery = bandResourceParameters.SearchQuery
                    });
                case UriType.Current:
                default:
                    return Url.Link("GetBands", new
                    {
                        fields = bandResourceParameters.Fields,
                        orderBy = bandResourceParameters.OrderBy,
                        pageNumber = bandResourceParameters.PageNumber,
                        pageSize = bandResourceParameters.PageSize,
                        mainGenre = bandResourceParameters.MainGenre,
                        searchQuery = bandResourceParameters.SearchQuery
                    });
            }
        }

        private IEnumerable<LinkDto> CreateLinksForBand(Guid bandId, string fields)
        { 
            var links = new List<LinkDto>();

            if (string.IsNullOrWhiteSpace(fields))
            {
                links.Add(
                    new LinkDto(
                        Url.Link("GetBand", new { bandId }),
                        "self",
                        "GET"));
            }
            else
            {
                links.Add(
                    new LinkDto(
                        Url.Link("GetBand", new { bandId, fields }),
                        "self",
                        "GET"));
            }

            links.Add(
                new LinkDto(
                    Url.Link("DeleteBand", new { bandId }),
                    "delete_band",
                    "DELETE"));
            
            links.Add(
                new LinkDto(
                    Url.Link("CreateAlbumForBand", new { bandId }),
                    "create_album_for_band",
                    "POST"));

            links.Add(
                new LinkDto(
                    Url.Link("GetAlbumsForBand", new { bandId }),
                    "albums",
                    "GET"));

            return links;
        }

        private IEnumerable<LinkDto> CreateLinksForBands(BandResourceParameters bandResourceParameters, bool hasNext, bool hasPrevious)
        {
            var links = new List<LinkDto>();

            links.Add(
                new LinkDto(
                    CreateBandsUri(bandResourceParameters, UriType.Current),
                    "self",
                    "GET"));

            if (hasNext)
            {
                links.Add(
                    new LinkDto(
                        CreateBandsUri(bandResourceParameters, UriType.NextPage),
                        "nextPage",
                        "GET"));
            }

            if (hasPrevious)
            {
                links.Add(
                    new LinkDto(
                        CreateBandsUri(bandResourceParameters, UriType.PreviousPage),
                        "PreviousPage",
                        "GET"));
            }

            return links;
        }
    }
}
