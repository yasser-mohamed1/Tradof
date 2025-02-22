namespace Tradof.Project.Services.DTOs
{
    public record LanguageDto(
        long Id,
        string LanguageName,
        string LanguageCode,
        string CountryName,
        string CountryCode
    );
}
