using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using Azure.Core;
using InvoicesCRUD.Core.DTO;
using InvoicesCRUD.Core.ServiceContracts;
using InvoicesCRUD.UI.Filters.ActionFilters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace InvoicesCRUD.UI.Controllers
{
    [Route("[controller]")]
    public class InvoicesController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly IInvoicesService _invoicesService;
        private readonly IInvoiceProductsService _invoiceProductsService;

        public InvoicesController(IConfiguration configuration, IInvoicesService invoicesService, IInvoiceProductsService invoiceProductsService)
        {
            _configuration = configuration;
            _invoicesService = invoicesService;
            _invoiceProductsService = invoiceProductsService;
        }

        [HttpGet]
        [Route("[action]")]
        [Route("")]
        [Route("~/")]
        public async Task<IActionResult> Index()
        {
            var invoices = await _invoicesService.GetInvoices();

            ViewBag.Invoices = invoices;

            return View();
        }

        [HttpGet]
        [Route("[action]")]
        public IActionResult Create()
        {
            InvoiceAddRequest invoiceAddRequest = new InvoiceAddRequest();

            return View(invoiceAddRequest);
        }

        [HttpPost]
        [Route("[action]")]
        [TypeFilter(typeof(CreateAndEditInvoiceActionFilter))]
        public async Task<IActionResult> Create(InvoiceAddRequest request)
        {
            InvoiceResponse invoice = await _invoicesService.AddInvoice(request);

            return RedirectToAction("Detail", new { id = invoice.InvoiceId });
        }

        [HttpGet]
        [Route("[action]/{id:Guid}")]
        public async Task<IActionResult> Edit(Guid id)
        {
            var invoice = await _invoicesService.GetInvoiceByInvoiceId(id);

            if (invoice == null)
            {
                return RedirectToAction("Index", "Invoices");
            }

            InvoiceUpdateRequest? updateRequest = invoice.ToInvoiceUpdateRequest();

            return View(updateRequest);
        }

        [HttpPost]
        [Route("[action]/{id:Guid}")]
        [TypeFilter(typeof(CreateAndEditInvoiceActionFilter))]
        public async Task<IActionResult> Edit(InvoiceUpdateRequest request)
        {
            var invoice = await _invoicesService.GetInvoiceByInvoiceId(request.InvoiceId);

            if (invoice == null)
            {
                return RedirectToAction("Index", "Invoices");
            }

            await _invoicesService.UpdateInvoice(request);

            return RedirectToAction("Detail", new { id = invoice.InvoiceId });
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> Delete(InvoiceUpdateRequest request)
        {
            bool deleted = await _invoicesService.DeleteInvoiceByInvoiceId(request.InvoiceId);

            if (!deleted)
            {
                return BadRequest();
            }

            return RedirectToAction("Index", "Invoices");
        }

        [HttpGet]
        [Route("[action]/{id:Guid}")]
        public async Task<IActionResult> Detail(Guid id)
        {
            InvoiceResponse? invoice = await _invoicesService.GetInvoiceByInvoiceId(id);

            if (invoice == null)
            {
                return RedirectToAction("Index", "Invoices");
            }

            return View(invoice);
        }

        [HttpPost]
        [Route("[action]")]
        [TypeFilter(typeof(FilterResultsActionFilter))]
        public async Task<IActionResult> FilterResults(InvoiceFilters? filters)
        {
            var invoices = await _invoicesService.GetInvoices(filters);

            ViewBag.Invoices = invoices;

            return View("Index", filters);
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> ExportExcel(Guid[] selectedInvoice)
        {
            List<Guid> invoiceIds = selectedInvoice.ToList();

            MemoryStream memoryStream = await _invoicesService.GetInvoicesExcel(invoiceIds);

            return File(memoryStream, "application/vnd.ms-excel", "invoices.xlsx");
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> AddInvoiceProduct(InvoiceProductAddRequest request)
        {
            var invoice = await _invoicesService.GetInvoiceByInvoiceId(request.InvoiceId);

            if (invoice == null)
            {
                return RedirectToAction("Index", "Invoices");
            }

            await _invoiceProductsService.AddInvoiceProduct(request);

            return RedirectToAction("Detail", new { id = invoice.InvoiceId });
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> UpdateInvoiceProduct(InvoiceProductUpdateRequest request)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("Detail", new { id = request.InvoiceId });
            }

            await _invoiceProductsService.UpdateInvoiceProduct(request);

            return RedirectToAction("Detail", new { id = request.InvoiceId });
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> DeleteInvoiceProduct([FromForm] string? InvoiceProductId, [FromForm] string? invoiceId)
        {
            await _invoiceProductsService.DeleteInvoiceProduct(Guid.Parse(InvoiceProductId));

            return RedirectToAction("Detail", new { id = Guid.Parse(invoiceId) });
        }

        [HttpGet]
        [Route("[action]/{id}")]
        public async Task<IActionResult> Print(Guid? id)
        {
            InvoiceResponse? invoice = await _invoicesService.GetInvoiceByInvoiceId(id);

            if (invoice == null)
            {
                return RedirectToAction("Index", "Invoices");
            }

            ViewBag.CompanyName = _configuration["CompanyInfomations:CompanyName"];
            ViewBag.CompanyAddress = _configuration["CompanyInfomations:CompanyAddress"];

            return View(invoice);
        }

    }
}

