using System;
using System.ComponentModel.DataAnnotations;
using InvoicesCRUD.Core.Enums;

namespace InvoicesCRUD.Core.Domain.Entities
{
    public class RunningNumber
    {
        [Key]
        [MaxLength(20)]
        public string RunningNumberType { get; set; }

        [MaxLength(3)]
        public string Prefix { get; set; }

        public int CurrentRunning { get; set; }
    }
}

