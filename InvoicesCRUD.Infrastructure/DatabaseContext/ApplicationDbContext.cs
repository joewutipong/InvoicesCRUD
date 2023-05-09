using System;
using System.Diagnostics.Metrics;
using InvoicesCRUD.Core.Domain.Entities;
using InvoicesCRUD.Core.Enums;
using Microsoft.EntityFrameworkCore;

namespace InvoicesCRUD.Infrastructure.DatabaseContext
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
        }

        public virtual DbSet<Invoice> Invoices { get; set; }
        public virtual DbSet<InvoiceProduct> InvoiceProducts { get; set; }
        public virtual DbSet<RunningNumber> RunningNumbers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<RunningNumber>().ToTable("RunningNumbers");

            List<RunningNumber> runningNumbers = new List<RunningNumber>()
            {
                new RunningNumber()
                {
                    RunningNumberType = RunningNumberTypes.Invoice.ToString(),
                    Prefix = "IV"
                }
            };

            modelBuilder.Entity<RunningNumber>().HasData(runningNumbers);
        }
    }
}

