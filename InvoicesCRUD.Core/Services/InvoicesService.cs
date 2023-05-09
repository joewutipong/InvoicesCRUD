using System;
using System.Linq;
using System.Linq.Expressions;
using InvoicesCRUD.Core.Domain.Entities;
using InvoicesCRUD.Core.Domain.RepositoryContracts;
using InvoicesCRUD.Core.DTO;
using InvoicesCRUD.Core.Enums;
using InvoicesCRUD.Core.Helpers;
using InvoicesCRUD.Core.ServiceContracts;
using OfficeOpenXml;

namespace InvoicesCRUD.Core.Services
{
    public class InvoicesService : IInvoicesService
    {
        private readonly IInvoicesRepository _invoicesRepository;
        private readonly IRunningNumbersRepository _runningNumbersRepository;

        public InvoicesService(IInvoicesRepository invoicesRepository,
            IRunningNumbersRepository runningNumbersRepository)
        {
            _invoicesRepository = invoicesRepository;
            _runningNumbersRepository = runningNumbersRepository;
        }

        #region Invoice
        public async Task<InvoiceResponse> AddInvoice(InvoiceAddRequest? request)
        {
            if (request == null)
            {
                throw new ArgumentNullException();
            }

            ValidationHelper.ModelValidation(request);

            Invoice invoice = request.ToInvoice();
            invoice.InvoiceNumber = await GenerateInvoiceNumber();

            Invoice invoiceAdded = await _invoicesRepository.AddInvoice(invoice);

            return invoiceAdded.ToInvoiceResponse();
        }

        public async Task<bool> DeleteInvoiceByInvoiceId(Guid? invoiceId)
        {
            if (invoiceId == null)
            {
                return false;
            }

            await _invoicesRepository.DeleteInvoiceByInvoiceId(invoiceId.Value);

            return true;
        }

        public async Task<List<InvoiceResponse>> GetInvoices()
        {
            List<Invoice> invoices = await _invoicesRepository.GetAllInvoices();

            return invoices.Select(temp => temp.ToInvoiceResponse()).ToList();
        }

        public async Task<List<InvoiceResponse>> GetInvoices(InvoiceFilters? filters)
        {
            ValidationHelper.ModelValidation(filters);

            List<Invoice> invoices = new List<Invoice>();
            if (filters == null)
            {
                invoices.AddRange(await _invoicesRepository.GetAllInvoices());
            }
            else
            {
                invoices.AddRange(await _invoicesRepository.GetFilteredInvoices(filters));
            }

            return invoices.Select(temp => temp.ToInvoiceResponse()).ToList();
        }

        public async Task<InvoiceResponse?> GetInvoiceByInvoiceId(Guid? invoiceId)
        {
            if (invoiceId == null)
            {
                throw new ArgumentNullException();
            }

            Invoice? invoice = await _invoicesRepository.GetInvoiceByInvoiceId(invoiceId.Value);

            if (invoice == null)
            {
                return null;
            }

            return invoice.ToInvoiceResponse();
        }

        public async Task<InvoiceResponse?> UpdateInvoice(InvoiceUpdateRequest? request)
        {
            if (request == null)
            {
                throw new ArgumentNullException();
            }

            ValidationHelper.ModelValidation(request);

            Invoice? matchingInvoice = await _invoicesRepository.GetInvoiceByInvoiceId(request.InvoiceId);

            if (matchingInvoice == null)
            {
                throw new ArgumentException();
            }

            matchingInvoice.DueDate = request.DueDate;
            matchingInvoice.CustomerName = request.CustomerName;
            matchingInvoice.CustomerAddress = request.CustomerAddress;

            await _invoicesRepository.UpdateInvoice(matchingInvoice);

            return matchingInvoice.ToInvoiceResponse();
        }

        public async Task<string?> GenerateInvoiceNumber()
        {
            RunningNumber? runningNumber = await _runningNumbersRepository.GetRunningNumber(RunningNumberTypes.Invoice);

            if (runningNumber == null)
            {
                throw new Exception("Can't generate invoice running number.");
            }

            await _runningNumbersRepository.IncreaseRunningNumber(RunningNumberTypes.Invoice);

            return RunningNumberHelper.GetNextRunningNumber(runningNumber.Prefix, runningNumber.CurrentRunning);
        }
        #endregion

        #region InvoiceProduct


        #endregion

        #region Export
        public async Task<MemoryStream> GetInvoicesExcel(List<Guid> invoiceIds)
        {
            MemoryStream memoryStream = new MemoryStream();

            using (ExcelPackage excelPackage = new ExcelPackage(memoryStream))
            {
                ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.Add("InvoiceSheet");

                worksheet.Cells["A1"].Value = "Invoice Number";
                worksheet.Cells["B1"].Value = "Due Date";
                worksheet.Cells["C1"].Value = "Customer Name";
                worksheet.Cells["D1"].Value = "Customer Address";
                worksheet.Cells["E1"].Value = "Total Price";

                using (ExcelRange headerCells = worksheet.Cells["A1:E1"])
                {
                    headerCells.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    headerCells.Style.Fill.BackgroundColor
                        .SetColor(System.Drawing.Color.LightGray);
                    headerCells.Style.Font.Bold = true;
                };

                int row = 2;

                List<Invoice> invoices = await _invoicesRepository.GetInvoicesByInvoiceIds(invoiceIds);

                foreach (Invoice invoice in invoices)
                {
                    worksheet.Cells[row, 1].Value = invoice.InvoiceNumber;
                    if (invoice.DueDate.HasValue)
                    {
                        worksheet.Cells[row, 2].Value = invoice.DueDate.Value.ToString("MM/dd/yyyy");
                    }
                    worksheet.Cells[row, 3].Value = invoice.CustomerName;
                    worksheet.Cells[row, 4].Value = invoice.CustomerAddress;
                    worksheet.Cells[row, 5].Value = invoice.TotalPrice;

                    row++;
                }

                worksheet.Cells[$"A1:H{row}"].AutoFitColumns();

                await excelPackage.SaveAsync();

                memoryStream.Position = 0;

                return memoryStream;
            }
        }
        #endregion
    }
}

