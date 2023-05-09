using System;
using InvoicesCRUD.Core.Domain.Entities;

namespace InvoicesCRUD.Core.Domain.RepositoryContracts
{
    public interface IInvoiceProductsRepository
    {
        Task AddInvoiceProduct(InvoiceProduct invoiceProduct);

        Task<InvoiceProduct?> GetInvoiceProductById(Guid invoiceProduct);

        Task DeleteInvoiceProductById(Guid invoiceProductId);

        Task UpdateInvoiceProduct(InvoiceProduct invoiceProductId);
    }
}

