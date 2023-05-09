using System;
namespace InvoicesCRUD.Core.Helpers
{
    public class RunningNumberHelper
    {
        public static string GetNextRunningNumber(string prefix, int lastNumber)
        {
            var counter = lastNumber + 1;
            var counterString = counter.ToString().PadLeft(5, '0');
            var runningNumber = $"{prefix}{counterString}";

            return runningNumber;
        }
    }
}

