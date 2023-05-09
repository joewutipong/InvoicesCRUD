using System;
namespace InvoicesCRUD.UI.ViewModels
{
    public class InvoiceFilters
    {
        public string? InvoiceNumber { get; set; } = string.Empty;
        public string? CustomerName { get; set; } = string.Empty;
        public DateTime? DueDate { get; set; } = null;
    }
}

