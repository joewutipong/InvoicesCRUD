using System;
using InvoicesCRUD.UI.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using static NuGet.Packaging.PackagingConstants;

namespace InvoicesCRUD.UI.Filters.ActionFilters
{
    public class FilterResultsActionFilter : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (context.Controller is InvoicesController invoicesController)
            {
                if (!invoicesController.ModelState.IsValid)
                {
                    var filters = context.ActionArguments["filters"];
                    context.Result = invoicesController.View("Index", filters);
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

