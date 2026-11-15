using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using MinhaEstante.Application.Exceptions;
using MinhaEstante.Domain.Exceptions;

namespace MinhaEstante.Api.Handlers
{
    public class GlobalExceptionHandler : IExceptionHandler
    {
        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            int? status = exception switch
            {
                DomainException => StatusCodes.Status400BadRequest,
                NotFoundException => StatusCodes.Status404NotFound,
                _ => null
            };

            if (status is null)
                return false;

            httpContext.Response.StatusCode = status.Value;
            await httpContext.Response.WriteAsJsonAsync(new ProblemDetails
            {
                Status = status,
                Title = "Não foi possível concluir a operação.",
                Detail = exception.Message
            }, cancellationToken);

            return true;
        }
    }
}
