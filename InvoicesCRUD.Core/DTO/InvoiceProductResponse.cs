using System;
using InvoicesCRUD.Core.Domain.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InvoicesCRUD.Core.DTO
{
    public class InvoiceProductResponse
    {
        public Guid InvoiceProductId { get; set; }

        public Guid InvoiceId { get; set; }

        public string? ProductName { get; set; }

        public double ProductPrice { get; set; }

        public int Quantity { get; set; }

        public double TotalPrice { get; set; }

        public InvoiceProductUpdateRequest ToInvoiceProductUpdateRequest()
        {
            return new InvoiceProductUpdateRequest()
            {
                InvoiceProductId = InvoiceProductId,
                InvoiceId = InvoiceId,
                ProductName = ProductName,
                ProductPrice = ProductPrice,
                Quantity = Quantity,
                TotalPrice = TotalPrice
            };
        }
    }

    public static class InvoiceProductExtension
    {
        public static InvoiceProductResponse ToInvoiceProductResponse(this InvoiceProduct invoiceProduct)
        {
            return new InvoiceProductResponse()
            {
                InvoiceId = invoiceProduct.InvoiceId,
                InvoiceProductId = invoiceProduct.InvoiceProductId,
                ProductName = invoiceProduct.ProductName,
                ProductPrice = invoiceProduct.ProductPrice,
                Quantity = invoiceProduct.Quantity,
                TotalPrice = invoiceProduct.TotalPrice
            };
        }
    }
}

