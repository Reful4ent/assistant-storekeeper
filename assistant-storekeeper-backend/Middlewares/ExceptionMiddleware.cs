using System;
using System.Text.Json;
using System.Threading.Tasks;
using assistant_storekeeper_backend.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace assistant_storekeeper_backend.Middlewares
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (NotFoundException ex)
            {
                _logger.LogError(ex, "Not found");
                context.Response.StatusCode = 404;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync(JsonSerializer.Serialize(new { error = ex.Message }));
            }
            catch (BadRequestException ex)
            {
                _logger.LogError(ex, "Bad request");
                context.Response.StatusCode = 400;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync(JsonSerializer.Serialize(new { error = ex.Message }));
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database update error");
                context.Response.ContentType = "application/json";
                if (ex.InnerException is PostgresException pg)
                {
                    if (pg.SqlState == "23505")
                    {
                        context.Response.StatusCode = 409;
                        await context.Response.WriteAsync(JsonSerializer.Serialize(new { error = "Unique constraint violation" }));
                    }
                    else if (pg.SqlState == "22001")
                    {
                        context.Response.StatusCode = 400;
                        await context.Response.WriteAsync(JsonSerializer.Serialize(new { error = pg.MessageText }));
                    }
                    else
                    {
                        context.Response.StatusCode = 400;
                        await context.Response.WriteAsync(JsonSerializer.Serialize(new { error = "Database update error" }));
                    }
                }
                else
                {
                    await context.Response.WriteAsync(JsonSerializer.Serialize(new { error = "Database update error" }));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled error");
                context.Response.StatusCode = 500;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync(JsonSerializer.Serialize(new { error = "Internal server error" }));
            }
        }
    }
}