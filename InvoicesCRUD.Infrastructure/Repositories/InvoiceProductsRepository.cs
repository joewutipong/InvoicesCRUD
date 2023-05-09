using System;
using InvoicesCRUD.Core.Domain.Entities;
using InvoicesCRUD.Core.Domain.RepositoryContracts;
using InvoicesCRUD.Infrastructure.DatabaseContext;
using Microsoft.EntityFrameworkCore;

namespace InvoicesCRUD.Infrastructure.Repositories
{
    public class InvoiceProductsRepository : IInvoiceProductsRepository
    {
        private readonly ApplicationDbContext _db;

        public InvoiceProductsRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task AddInvoiceProduct(InvoiceProduct invoiceProduct)
        {
            _db.InvoiceProducts.Add(invoiceProduct);

            await _db.SaveChangesAsync();
        }

        public async Task<InvoiceProduct?> GetInvoiceProductById(Guid invoiceProductId)
        {
            return await _db.InvoiceProducts.FirstOrDefaultAsync(temp => temp.InvoiceProductId == invoiceProductId);
        }

        public async Task UpdateInvoiceProduct(InvoiceProduct invoiceProduct)
        {
            InvoiceProduct? matchingInvoice = await _db.InvoiceProducts
                .FirstOrDefaultAsync(temp => temp.InvoiceProductId == invoiceProduct.InvoiceProductId);

            if (matchingInvoice != null)
            {
                matchingInvoice.ProductName = invoiceProduct.ProductName;
                matchingInvoice.ProductPrice = invoiceProduct.ProductPrice;
                matchingInvoice.Quantity = invoiceProduct.Quantity;
                matchingInvoice.TotalPrice = invoiceProduct.ProductPrice * invoiceProduct.Quantity;

                await _db.SaveChangesAsync();
            }
        }

        public async Task DeleteInvoiceProductById(Guid invoiceProductId)
        {
            InvoiceProduct? matchingInvoiceProduct = await _db.InvoiceProducts
                .FirstOrDefaultAsync(temp => temp.InvoiceProductId == invoiceProductId);

            if (matchingInvoiceProduct != null)
            {
                Guid invoiceId = matchingInvoiceProduct.InvoiceId;

                _db.InvoiceProducts.Remove(matchingInvoiceProduct);

                await _db.SaveChangesAsync();
            }
        }
    }
}

