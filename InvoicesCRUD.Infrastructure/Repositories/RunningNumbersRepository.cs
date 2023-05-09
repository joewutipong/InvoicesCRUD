using System;
using InvoicesCRUD.Core.Domain.Entities;
using InvoicesCRUD.Core.Domain.RepositoryContracts;
using InvoicesCRUD.Core.Enums;
using InvoicesCRUD.Infrastructure.DatabaseContext;
using Microsoft.EntityFrameworkCore;

namespace InvoicesCRUD.Infrastructure.Repositories
{
    public class RunningNumbersRepository : IRunningNumbersRepository
    {
        private readonly ApplicationDbContext _db;

        public RunningNumbersRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<RunningNumber?> GetRunningNumber(RunningNumberTypes types)
        {
            return await _db.RunningNumbers.FirstOrDefaultAsync(temp => temp.RunningNumberType == types.ToString());
        }

        public async Task IncreaseRunningNumber(RunningNumberTypes types)
        {
            RunningNumber? matching = await _db.RunningNumbers.FirstOrDefaultAsync(temp => temp.RunningNumberType == types.ToString());

            if (matching != null)
            {
                matching.CurrentRunning += 1;

                await _db.SaveChangesAsync();
            }
        }
    }
}

