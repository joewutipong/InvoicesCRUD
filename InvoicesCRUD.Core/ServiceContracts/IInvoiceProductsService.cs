using System;
using InvoicesCRUD.Core.DTO;

namespace InvoicesCRUD.Core.ServiceContracts
{
    public interface IInvoiceProductsService
    {

        Task AddInvoiceProduct(InvoiceProductAddRequest? request);

        Task UpdateInvoiceProduct(InvoiceProductUpdateRequest? request);

        Task DeleteInvoiceProduct(Guid? invoiceProductId);
    }
}

