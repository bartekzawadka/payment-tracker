using System;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Payment.Tracker.Api.Extensions;
using Payment.Tracker.Api.Models;
using Payment.Tracker.BusinessLogic.ServiceAction;

namespace Payment.Tracker.Api.Filters
{
    public class ServiceActionFilter : IActionFilter
    {
        public void OnActionExecuted(ActionExecutedContext context)
        {
            if (!(context.Result is ObjectResult result) || result.Value == null)
            {
                return;
            }

            context.Result = result.Value switch
            {
                IServiceActionResult<int> intValue => GetActionResult(intValue),
                IServiceActionResult<bool> boolValue => GetActionResult(boolValue),
                IServiceActionResult<object> objectValue => GetActionResult(objectValue),
                IServiceActionResult value => GetActionResult(value),
                _ => context.Result
            };
        }

        private static IActionResult GetActionResult(IServiceActionResult serviceActionResult) =>
            new ObjectResult(new ErrorResult(serviceActionResult.ErrorMessages).ToJson())
            {
                StatusCode = GetStatusCodeForResultType(serviceActionResult.ResultType)
            };

        private static IActionResult GetActionResult<T>(IServiceActionResult<T> serviceActionResult) =>
            new ObjectResult(serviceActionResult.GetData()
                             ?? new ErrorResult(serviceActionResult.ErrorMessages).ToJson())
            {
                StatusCode = GetStatusCodeForResultType(serviceActionResult.ResultType)
            };

        private static int GetStatusCodeForResultType(ServiceActionResultType serviceActionResultType) =>
            serviceActionResultType switch
            {
                ServiceActionResultType.Created => (int) HttpStatusCode.Created,
                ServiceActionResultType.Success => (int) HttpStatusCode.OK,
                ServiceActionResultType.ForbiddenAccess => (int) HttpStatusCode.Forbidden,
                ServiceActionResultType.UnauthorizedAccess => (int) HttpStatusCode.Unauthorized,
                ServiceActionResultType.InvalidDataOrOperation => (int) HttpStatusCode.BadRequest,
                ServiceActionResultType.ObjectNotFound => (int) HttpStatusCode.NotFound,
                _ => throw new InvalidOperationException("Nieobsługiwany typ wyniku operacji")
            };

        public void OnActionExecuting(ActionExecutingContext context)
        {
            if (context.ModelState.IsValid)
            {
                return;
            }

            context.Result = context.ModelState.ToBadResponseResult();
        }
    }
}