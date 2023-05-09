using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.VisualBasic;

namespace InvoicesCRUD.Core.DTO
{
    public class InvoiceFilters : IValidatableObject
    {
        public string? InvoiceNumber { get; set; } = string.Empty;
        public string? CustomerName { get; set; } = string.Empty;
        public DateTime? FromDueDate { get; set; } = null;
        public DateTime? ToDueDate { get; set; } = null;

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (FromDueDate != null || ToDueDate != null)
            {
                if (FromDueDate > ToDueDate)
                {
                    yield return new ValidationResult("'To Due Date' should be newer than or equal to 'To Due Date'", new[] { nameof(ToDueDate) });
                }
            }
        }
    }
}

