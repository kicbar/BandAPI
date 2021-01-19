using System;
using System.ComponentModel.DataAnnotations;

namespace BandAPI.Models
{
    public class AlbumForCreatingDto
    {
        [Required]
        [MaxLength(200)]
        public string Title { get; set; }
        [MaxLength(400)]
        public string Description { get; set; }
    }
}
