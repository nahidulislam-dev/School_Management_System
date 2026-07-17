using FluentValidation;
using SchoolERP.Application.Common.Exceptions;

namespace SchoolERP.Api.Middleware
{
    // Api/Middleware/ExceptionHandlingMiddleware.cs
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        public ExceptionHandlingMiddleware(RequestDelegate next) => _next = next;

        public async Task InvokeAsync(HttpContext context)
        {
            try { await _next(context); }
            catch (NotFoundException ex)
            {
                context.Response.StatusCode = StatusCodes.Status404NotFound;
                await context.Response.WriteAsJsonAsync(new { success = false, message = ex.Message });
            }
            catch (BadRequestException ex)
            {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                await context.Response.WriteAsJsonAsync(new { success = false, message = ex.Message });
            }
            catch (ValidationException ex) // FluentValidation
            {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                await context.Response.WriteAsJsonAsync(new { success = false,
                    errors = ex.Errors.Select(x => new
                    {
                        field = x.PropertyName,
                        message = x.ErrorMessage
                    })
                });

            }
            catch (InvalidOperationException ex)
            {
                context.Response.StatusCode =
                    StatusCodes.Status400BadRequest;

                await context.Response.WriteAsJsonAsync(
                    new
                    {
                        success = false,
                        message = ex.Message
                    });
            }
            catch (Exception ex)
            {
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                await context.Response.WriteAsJsonAsync(new { success = false, message = "Unexpected error" });
            }
        }
    }
}
