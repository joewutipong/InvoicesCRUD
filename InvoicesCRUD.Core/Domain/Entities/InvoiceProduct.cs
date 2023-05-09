using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Xml.Linq;

namespace InvoicesCRUD.Core.Domain.Entities
{
    public class InvoiceProduct
    {
        [Key]
        public Guid InvoiceProductId { get; set; }

        [ForeignKey(nameof(Invoice))]
        public Guid InvoiceId { get; set; }
        public Invoice Invoice { get; }

        [MaxLength(200)]
        public string? ProductName { get; set; }

        public double ProductPrice { get; set; }

        public int Quantity { get; set; }

        public double TotalPrice { get; set; }
    }
}

