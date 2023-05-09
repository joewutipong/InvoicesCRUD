using System;
using System.Linq.Expressions;
using InvoicesCRUD.Core.Domain.Entities;
using InvoicesCRUD.Core.DTO;

namespace InvoicesCRUD.Core.Domain.RepositoryContracts
{
    public interface IInvoicesRepository
    {
        Task<Invoice> AddInvoice(Invoice invoice);

        Task DeleteInvoiceByInvoiceId(Guid invoiceId);

        Task<List<Invoice>> GetAllInvoices();

        Task<Invoice?> GetInvoiceByInvoiceId(Guid invoiceId);

        Task<Invoice?> GetInvoiceByInvoiceNumber(string invoiceNumber);

        Task UpdateInvoice(Invoice invoice);

        Task<List<Invoice>> GetFilteredInvoices(InvoiceFilters invoiceFilters);

        Task UpdateInvoiceTotalPrice(Guid invoiceId);

        Task<List<Invoice>> GetInvoicesByInvoiceIds(List<Guid> invoiceIds);
    }
}

