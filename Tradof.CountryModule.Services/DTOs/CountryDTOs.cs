namespace Tradof.CountryModule.Services.DTOs
{
    public record CountryDto(
        long Id,
        string Name
        //DateTime CreationDate,
        //DateTime? ModificationDate,
        //string CreatedBy,
        //string ModifiedBy
        );

    public record CreateCountryDto(
        string Name);

    public record UpdateCountryDto(
        long Id,
        string Name,
        DateTime? ModificationDate,
        string ModifiedBy);
}
