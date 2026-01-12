using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace TaskFlow.Api.Validations
{
    public class NoAdminAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value != null)
            {
                var name = value.ToString().ToLower();
                if (name.Contains("admin"))
                {
                    return new ValidationResult("Tên không được phép chứa từ nhạy cảm 'admin'!");
                }
            }

            return ValidationResult.Success;
        }
    }
}
