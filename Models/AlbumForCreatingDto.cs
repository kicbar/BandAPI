using BandAPI.Validators;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BandAPI.Models
{
    [TitleAndDescription(ErrorMessage = "Title must Be Different From Description")]
    public class AlbumForCreatingDto
    {
        [Required(ErrorMessage = "Title needs to be field in")]
        [MaxLength(200, ErrorMessage = "Title needs to be up to 200 characters")]
        public string Title { get; set; }
        [MaxLength(400, ErrorMessage = "Title needs to be up to 200 characters")]
        public string Description { get; set; }
    }
}
