using BandAPI.Validators;
using System.ComponentModel.DataAnnotations;

namespace BandAPI.Models
{
    [TitleAndDescription(ErrorMessage = "Title must Be Different From Description")]
    public abstract class AlbumManipulationDto
    {
        [Required(ErrorMessage = "Title needs to be field in")]
        [MaxLength(200, ErrorMessage = "Title needs to be up to 200 characters")]
        public string Title { get; set; }
        [MaxLength(400, ErrorMessage = "Title needs to be up to 400 characters")]
        public virtual string Description { get; set; }
    }
}
