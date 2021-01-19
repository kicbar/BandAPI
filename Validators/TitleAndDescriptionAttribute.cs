using BandAPI.Models;
using System.ComponentModel.DataAnnotations;

namespace BandAPI.Validators
{
    public class TitleAndDescriptionAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var album = (AlbumForCreatingDto)validationContext.ObjectInstance;

            if (album.Title == album.Description)
            {
                return new ValidationResult("The title and the description need to be different.", new[] { "AlbumForCreatingDto" });
            }

            return ValidationResult.Success;
        }
    }
}
