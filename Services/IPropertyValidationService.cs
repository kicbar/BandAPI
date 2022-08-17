namespace BandAPI.Services
{
    public interface IPropertyValidationService
    {
        bool HasValidProperty<T>(string fields);
    }
}