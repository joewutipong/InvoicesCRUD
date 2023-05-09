using System;
using System.Linq.Expressions;
using InvoicesCRUD.Core.Domain.Entities;
using InvoicesCRUD.Core.Domain.RepositoryContracts;
using InvoicesCRUD.Core.DTO;
using InvoicesCRUD.Infrastructure.DatabaseContext;
using Microsoft.EntityFrameworkCore;

namespace InvoicesCRUD.Infrastructure.Repositories
{
    public class InvoicesRepository : IInvoicesRepository
    {
        private readonly ApplicationDbContext _db;

        public InvoicesRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<Invoice> AddInvoice(Invoice invoice)
        {
            _db.Invoices.Add(invoice);

            await _db.SaveChangesAsync();

            return invoice;
        }

        public async Task DeleteInvoiceByInvoiceId(Guid invoiceId)
        {
            Invoice? matchingInvoice = await _db.Invoices.FirstOrDefaultAsync(temp => temp.InvoiceId == invoiceId);

            if (matchingInvoice != null)
            {
                _db.Invoices.Remove(matchingInvoice);

                await _db.SaveChangesAsync();
            }
        }

        public async Task<List<Invoice>> GetAllInvoices()
        {
            return await _db.Invoices.Include(temp => temp.InvoiceProducts)
                .OrderBy(temp => temp.InvoiceNumber)
                .ToListAsync();
        }

        public async Task<List<Invoice>> GetFilteredInvoices(InvoiceFilters filters)
        {
            var invoices = _db.Invoices.AsNoTracking();

            if (!String.IsNullOrEmpty(filters.InvoiceNumber))
            {
                invoices = invoices.Where(temp => temp.InvoiceNumber.Contains(filters.InvoiceNumber));
            }

            if (!String.IsNullOrEmpty(filters.CustomerName))
            {
                invoices = invoices.Where(temp => temp.CustomerName.Contains(filters.CustomerName));
            }

            if (filters.FromDueDate != null)
            {
                invoices = invoices.Where(temp => temp.DueDate >= filters.FromDueDate);
            }

            if (filters.ToDueDate != null)
            {
                invoices = invoices.Where(temp => temp.DueDate <= filters.ToDueDate);
            }

            return await invoices.OrderBy(temp => temp.InvoiceNumber).ToListAsync();
        }

        public async Task<Invoice?> GetInvoiceByInvoiceId(Guid invoiceId)
        {
            return await _db.Invoices.Include(temp => temp.InvoiceProducts).FirstOrDefaultAsync(temp => temp.InvoiceId == invoiceId);
        }

        public async Task<Invoice?> GetInvoiceByInvoiceNumber(string invoiceNumber)
        {
            return await _db.Invoices.Include(temp => temp.InvoiceProducts).FirstOrDefaultAsync(temp => temp.InvoiceNumber == invoiceNumber);
        }

        public async Task UpdateInvoice(Invoice invoice)
        {
            Invoice? matchingInvoice = await _db.Invoices.FirstOrDefaultAsync(temp => temp.InvoiceId == invoice.InvoiceId);

            if (matchingInvoice != null)
            {
                matchingInvoice.CustomerName = invoice.CustomerName;
                matchingInvoice.CustomerAddress = invoice.CustomerAddress;
                matchingInvoice.DueDate = invoice.DueDate;
                matchingInvoice.TotalPrice = invoice.TotalPrice;

                await _db.SaveChangesAsync();
            }
        }

        public async Task UpdateInvoiceTotalPrice(Guid invoiceId)
        {
            Invoice? invoice = await _db.Invoices.Include(temp => temp.InvoiceProducts).FirstOrDefaultAsync(temp => temp.InvoiceId == invoiceId);

            if (invoice != null)
            {
                invoice.TotalPrice = invoice.InvoiceProducts.Sum(temp => temp.TotalPrice);

                await _db.SaveChangesAsync();
            }
        }

        public async Task<List<Invoice>> GetInvoicesByInvoiceIds(List<Guid> invoiceIds)
        {
            return await _db.Invoices.Where(temp => invoiceIds.Contains(temp.InvoiceId))
                .OrderBy(temp => temp.InvoiceNumber)
                .ToListAsync();
        }
    }
}

