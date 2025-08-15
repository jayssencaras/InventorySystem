using System.ComponentModel.DataAnnotations;

namespace Inventory.Api.Validation;

public static class ValidationExtensions
{
    public static (bool isValid, Dictionary<string, string[]> errors) Validate<T>(this T model)
    {
        var ctx = new ValidationContext(model!);
        var results = new List<ValidationResult>();
        var ok = Validator.TryValidateObject(model!, ctx, results, validateAllProperties: true);
        var errors = results
            .GroupBy(r => r.MemberNames.FirstOrDefault() ?? string.Empty)
            .ToDictionary(g => g.Key, g => g.Select(r => r.ErrorMessage ?? "Invalid").ToArray());
        return (ok, errors);
    }
}
