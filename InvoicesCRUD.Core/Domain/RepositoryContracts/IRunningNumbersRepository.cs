using System;
using InvoicesCRUD.Core.Domain.Entities;
using InvoicesCRUD.Core.Enums;

namespace InvoicesCRUD.Core.Domain.RepositoryContracts
{
    public interface IRunningNumbersRepository
    {
        Task<RunningNumber?> GetRunningNumber(RunningNumberTypes types);

        Task IncreaseRunningNumber(RunningNumberTypes types);
    }
}

