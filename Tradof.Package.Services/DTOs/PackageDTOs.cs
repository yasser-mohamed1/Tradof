namespace Tradof.PackageNamespace.Services.DTOs
{
    public record PackageDto(
        long Id,
        string Name,
        string Description,
        double Price,
        int DurationInMonths,
        DateTime CreationDate,
        DateTime? ModificationDate,
        string CreatedBy,
        string ModifiedBy);

    public record CreatePackageDto(
        string Name,
        string Description,
        double Price,
        int DurationInMonths);

    public record UpdatePackageDto(
        long Id,
        string Name,
        string Description,
        double Price,
        int DurationInMonths,
        DateTime? ModificationDate,
        string ModifiedBy);
}