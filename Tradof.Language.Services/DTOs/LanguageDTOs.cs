namespace Tradof.Language.Services.DTOs
{
    public record LanguageDto(
        long Id,
        string Name,
        string Code
        //DateTime CreationDate,
        //DateTime? ModificationDate,
        //string CreatedBy,
        //string ModifiedBy
        );

    public record CreateLanguageDto(
        string Name,
        string Code);

    public record UpdateLanguageDto(
        long Id,
        string Name,
        string Code,
        DateTime? ModificationDate,
        string ModifiedBy);
}
