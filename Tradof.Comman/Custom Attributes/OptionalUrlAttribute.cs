using System.ComponentModel.DataAnnotations;

public class OptionalUrlAttribute : ValidationAttribute
{
    protected override ValidationResult IsValid(object? value, ValidationContext validationContext)
    {
        if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
        {
            return ValidationResult.Success; // Allow empty value
        }

        if (Uri.TryCreate(value.ToString(), UriKind.Absolute, out _))
        {
            return ValidationResult.Success;
        }

        return new ValidationResult("Invalid URL format.");
    }
}