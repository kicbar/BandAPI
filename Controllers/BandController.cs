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
    [Route("api/bands")]
    public class BandController : ControllerBase
    {
        private readonly IBandAlbumRepository _bandAlbumRepository;
        private readonly IMapper _mapper;

        public BandController(IBandAlbumRepository bandAlbumRepository, IMapper mapper)
        {
            _bandAlbumRepository = bandAlbumRepository ??
                throw new ArgumentNullException(nameof(bandAlbumRepository));
            _mapper = mapper ??
                throw new ArgumentNullException(nameof(mapper));
        }

        [HttpGet]
        [HttpHead]
        public ActionResult<IEnumerable<BandDto>> GetBands([FromQuery]string mainGenre)
        {
            var bands = _bandAlbumRepository.GetBands(mainGenre);

            return Ok(_mapper.Map<IEnumerable<BandDto>>(bands));
        }

        [HttpGet("{bandId}")]
        [HttpHead]
        public IActionResult GetBand(Guid bandId)
        {
            var band = _bandAlbumRepository.GetBand(bandId);

            if (band == null)
                return NotFound();

            return Ok(band);
        }


    }
}
