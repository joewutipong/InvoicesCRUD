using System;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;
using InvoicesCRUD.Core.DTO;
using Microsoft.VisualBasic;

namespace InvoicesCRUD.Core.Domain.Entities
{
    public class Invoice
    {
        [Key]
        public Guid InvoiceId { get; set; }

        [MaxLength(8)]
        public string? InvoiceNumber { get; set; }

        public DateTime? DueDate { get; set; }

        [MaxLength(200)]
        public string? CustomerName { get; set; }

        [MaxLength(400)]
        public string? CustomerAddress { get; set; }

        public double TotalPrice { get; set; }

        public ICollection<InvoiceProduct> InvoiceProducts { get; set; } = new List<InvoiceProduct>();
    }
}

