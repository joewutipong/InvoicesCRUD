using System;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;
using InvoicesCRUD.Core.Domain.Entities;

namespace InvoicesCRUD.Core.DTO
{
    public class InvoiceUpdateRequest : IValidatableObject
    {
        [Display(Name = "Invoice Id")]
        [Required]
        public Guid InvoiceId { get; set; }

        [Display(Name = "Invoice Number")]
        public string? InvoiceNumber { get; set; }

        [Display(Name = "Due Date")]
        [Required]
        public DateTime? DueDate { get; set; }

        [Display(Name = "Customer Name")]
        [Required]
        [MaxLength(200)]
        public string? CustomerName { get; set; }

        [Display(Name = "Customer Address")]
        [Required]
        [MaxLength(400)]
        public string? CustomerAddress { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (DueDate is null || DueDate < DateTime.Today)
            {
                yield return new ValidationResult("Due Date should be newer than or equal to today", new[] { nameof(DueDate) });
            }
        }

        public Invoice ToInvoice()
        {
            return new Invoice()
            {
                InvoiceId = InvoiceId,
                InvoiceNumber = InvoiceNumber,
                DueDate = DueDate,
                CustomerName = CustomerName,
                CustomerAddress = CustomerAddress,
            };
        }
    }
}

