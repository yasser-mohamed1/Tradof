namespace Tradof.SpecializationModule.Services.DTOs
{
    public record SpecializationDto(
        long Id,
        string Name
        //DateTime CreationDate,
        //DateTime? ModificationDate,
        //string CreatedBy,
        //string ModifiedBy
    );

    public record CreateSpecializationDto(
        string Name);

    public record UpdateSpecializationDto(
        long Id,
        string Name,
        DateTime? ModificationDate,
        string ModifiedBy);
}
