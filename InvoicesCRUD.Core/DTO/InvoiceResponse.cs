using System;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;
using InvoicesCRUD.Core.Domain.Entities;

namespace InvoicesCRUD.Core.DTO
{
    public class InvoiceResponse
    {
        public Guid InvoiceId { get; set; }

        public string? InvoiceNumber { get; set; }

        public DateTime? DueDate { get; set; }

        public string? CustomerName { get; set; }

        public string? CustomerAddress { get; set; }

        public double TotalPrice { get; set; }

        public List<InvoiceProductResponse>? InvoiceProducts { get; set; } = new List<InvoiceProductResponse>();

        public InvoiceUpdateRequest ToInvoiceUpdateRequest()
        {
            var response = new InvoiceUpdateRequest()
            {
                InvoiceId = InvoiceId,
                InvoiceNumber = InvoiceNumber,
                DueDate = DueDate,
                CustomerName = CustomerName,
                CustomerAddress = CustomerAddress
            };

            return response;
        }
    }

    public static class InvoiceExtensions
    {
        public static InvoiceResponse ToInvoiceResponse(this Invoice invoice)
        {
            var response = new InvoiceResponse()
            {
                InvoiceId = invoice.InvoiceId,
                InvoiceNumber = invoice.InvoiceNumber,
                DueDate = invoice.DueDate,
                CustomerName = invoice.CustomerName,
                CustomerAddress = invoice.CustomerAddress,
                TotalPrice = invoice.TotalPrice
            };

            foreach (var product in invoice.InvoiceProducts)
            {
                response.InvoiceProducts.Add(product.ToInvoiceProductResponse());
            }

            return response;
        }
    }
}

