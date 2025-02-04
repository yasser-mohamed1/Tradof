namespace Tradof.Language.Services.DTOs
{
	public record LanguageDto(
	  long Id,
		string LanguageName,
		string LanguageCode,
		string CountryName,
		string CountryCode
		);

	public record CreateLanguageDto(
		string LanguageName,
		string LanguageCode,
		string CountryName,
		string CountryCode
		);

	public record UpdateLanguageDto(
		long Id,
		string LanguageName,
		string LanguageCode,
	    string CountryName,
		string CountryCode,
		DateTime? ModificationDate,
		string ModifiedBy);
}
