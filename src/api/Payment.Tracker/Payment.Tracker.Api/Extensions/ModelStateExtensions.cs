using System.Linq;
using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Payment.Tracker.Api.Models;

namespace Payment.Tracker.Api.Extensions
{
    public static class ModelStateExtensions
    {
        public static IActionResult ToBadResponseResult(this ModelStateDictionary modelState)
        {
            var response = new BadRequestObjectResult(GetErrorResult(modelState).ToJson());
            response.ContentTypes.Add(MediaTypeNames.Application.Json);
            return response;
        }

        private static ErrorResult GetErrorResult(ModelStateDictionary modelState) =>
            new ErrorResult(modelState
                .Values
                .SelectMany(modelStateValue => modelStateValue.Errors)
                .Select(modelError => modelError.ErrorMessage));
    }
}