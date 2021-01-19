using BandAPI.Validators;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BandAPI.Models
{
    [TitleAndDescriptionAttribute]
    public class AlbumForCreatingDto //: IValidatableObject
    {
        [Required]
        [MaxLength(200)]
        public string Title { get; set; }
        [MaxLength(400)]
        public string Description { get; set; }

        /*public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Title == Description)
            { 
                yield return new ValidationResult("The title and description need to be different.", new[] {"AlbumForCreatingDto"});
            }
        }*/
    }
}
