using HuntleyWeb.Application.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using System;

namespace HuntleyWebAPI.Controllers
{
    [ApiController]
    [ApiExplorerSettings(IgnoreApi = true)]
    [AllowAnonymous]
    public class ErrorController : ControllerBase
    {
        [Route("/error")]
        public IActionResult Error([FromServices] IWebHostEnvironment webHostEnvironment)
        {
            var context = HttpContext.Features.Get<IExceptionHandlerFeature>();
            var exceptionType = context.Error.GetType();

            if (exceptionType == typeof(ArgumentException)
               || exceptionType == typeof(ArgumentNullException)
                || exceptionType == typeof(ArgumentOutOfRangeException))
            {
                return ValidationProblem(context.Error.StackTrace, title: context.Error.Message);
            }

            if (exceptionType == typeof(NotFoundException))
            {
                return NotFound(context.Error.Message);
            }

            if (webHostEnvironment.IsDevelopment())
            {
                return Problem(context.Error.StackTrace, title: context.Error.Message);
            }

            return Problem(title: context.Error.Message);
        }
    }
}
