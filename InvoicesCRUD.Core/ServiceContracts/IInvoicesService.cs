using System;
using InvoicesCRUD.Core.DTO;

namespace InvoicesCRUD.Core.ServiceContracts
{
    public interface IInvoicesService
    {
        Task<InvoiceResponse> AddInvoice(InvoiceAddRequest? request);

        Task<List<InvoiceResponse>> GetInvoices();

        Task<List<InvoiceResponse>> GetInvoices(InvoiceFilters filters);

        Task<InvoiceResponse?> GetInvoiceByInvoiceId(Guid? invoiceId);

        Task<InvoiceResponse?> UpdateInvoice(InvoiceUpdateRequest? request);

        Task<bool> DeleteInvoiceByInvoiceId(Guid? invoiceId);

        Task<string?> GenerateInvoiceNumber();

        Task<MemoryStream> GetInvoicesExcel(List<Guid> invoiceIds);
    }
}

