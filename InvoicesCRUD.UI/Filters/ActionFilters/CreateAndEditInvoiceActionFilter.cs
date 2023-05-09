using System;
using InvoicesCRUD.UI.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;

namespace InvoicesCRUD.UI.Filters.ActionFilters
{
    public class CreateAndEditInvoiceActionFilter : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (context.Controller is InvoicesController invoicesController)
            {
                if (!invoicesController.ModelState.IsValid)
                {
                    var request = context.ActionArguments["request"];
                    context.Result = invoicesController.View(request);
                }
                else
                {
                    await next();
                }
            }
            else
            {
                await next();
            }
        }
    }
}

