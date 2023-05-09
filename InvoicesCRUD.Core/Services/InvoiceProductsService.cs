using System;
using InvoicesCRUD.Core.Domain.Entities;
using InvoicesCRUD.Core.Domain.RepositoryContracts;
using InvoicesCRUD.Core.DTO;
using InvoicesCRUD.Core.Helpers;
using InvoicesCRUD.Core.ServiceContracts;

namespace InvoicesCRUD.Core.Services
{
    public class InvoiceProductsService : IInvoiceProductsService
    {
        private readonly IInvoicesRepository _invoicesRepository;
        private readonly IInvoiceProductsRepository _invoiceProductsRepository;

        public InvoiceProductsService(IInvoicesRepository invoicesRepository, IInvoiceProductsRepository invoiceProductsRepository)
        {
            _invoicesRepository = invoicesRepository;
            _invoiceProductsRepository = invoiceProductsRepository;
        }

        public async Task AddInvoiceProduct(InvoiceProductAddRequest? request)
        {
            if (request == null)
            {
                throw new ArgumentNullException();
            }

            ValidationHelper.ModelValidation(request);

            Invoice? invoice = await _invoicesRepository.GetInvoiceByInvoiceId(request.InvoiceId);

            if (invoice == null)
            {
                throw new ArgumentException();
            }
            InvoiceProduct invoiceProduct = request.ToInvoiceProduct();

            await _invoiceProductsRepository.AddInvoiceProduct(invoiceProduct);

            await _invoicesRepository.UpdateInvoiceTotalPrice(invoice.InvoiceId);
        }

        public async Task UpdateInvoiceProduct(InvoiceProductUpdateRequest? request)
        {
            if (request == null)
            {
                throw new ArgumentNullException();
            }

            ValidationHelper.ModelValidation(request);

            InvoiceProduct? matcingInvoiceProduct = await _invoiceProductsRepository.GetInvoiceProductById(request.InvoiceProductId);

            if (matcingInvoiceProduct == null)
            {
                throw new ArgumentException();
            }

            matcingInvoiceProduct.ProductName = request.ProductName;
            matcingInvoiceProduct.ProductPrice = request.ProductPrice;
            matcingInvoiceProduct.Quantity = request.Quantity;

            await _invoiceProductsRepository.UpdateInvoiceProduct(matcingInvoiceProduct);

            await _invoicesRepository.UpdateInvoiceTotalPrice(matcingInvoiceProduct.InvoiceId);
        }

        public async Task DeleteInvoiceProduct(Guid? invoiceProductId)
        {
            if (invoiceProductId == null)
            {
                throw new ArgumentNullException();
            }

            InvoiceProduct? matcingInvoiceProduct = await _invoiceProductsRepository.GetInvoiceProductById(invoiceProductId.Value);

            if (matcingInvoiceProduct == null)
            {
                throw new ArgumentException();
            }

            await _invoiceProductsRepository.DeleteInvoiceProductById(matcingInvoiceProduct.InvoiceProductId);

            await _invoicesRepository.UpdateInvoiceTotalPrice(matcingInvoiceProduct.InvoiceId);
        }
    }
}

