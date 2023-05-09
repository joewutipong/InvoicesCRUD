using System;
using System.ComponentModel.DataAnnotations;
using InvoicesCRUD.Core.Domain.Entities;

namespace InvoicesCRUD.Core.DTO
{
    public class InvoiceProductAddRequest
    {
        [Display(Name = "Invoice Id")]
        [Required]
        public Guid InvoiceId { get; set; }

        [Display(Name = "Product Name")]
        [Required]
        [MaxLength(200)]
        public string? ProductName { get; set; }

        [Display(Name = "Product Price")]
        [Required]
        [Range(minimum: 1, maximum: 1000000)]
        public double ProductPrice { get; set; }

        [Display(Name = "Product Quantity")]
        [Required]
        [Range(minimum: 1, maximum: 1000)]
        public int Quantity { get; set; }

        [Display(Name = "Product Total Price")]
        public double TotalPrice { get; set; }

        public InvoiceProduct ToInvoiceProduct()
        {
            return new InvoiceProduct()
            {
                InvoiceId = InvoiceId,
                ProductName = ProductName,
                ProductPrice = ProductPrice,
                Quantity = Quantity,
                TotalPrice = ProductPrice * Quantity
            };
        }
    }
}

