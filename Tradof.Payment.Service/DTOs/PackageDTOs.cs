namespace Tradof.PaymentNameSpace.Services.DTOs
{
    public record PaymentDto(
        long Id,
        string Name,
        string Description,
        double Price,
        int DurationInMonths,
        DateTime CreationDate,
        DateTime? ModificationDate,
        string CreatedBy,
        string ModifiedBy);

    public record CreatePaymentDto(
        string Name,
        string Description,
        double Price,
        int DurationInMonths);

    public record UpdatePaymentDto(
        long Id,
        string Name,
        string Description,
        double Price,
        int DurationInMonths,
        DateTime? ModificationDate,
        string ModifiedBy);
}