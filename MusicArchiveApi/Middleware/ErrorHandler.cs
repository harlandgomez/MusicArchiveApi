using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using MusicArchiveApi.Exceptions;
using Newtonsoft.Json;

namespace MusicArchiveApi.Middleware
{
    public class ErrorHandler
    {
        private readonly RequestDelegate _next;
        public ErrorHandler(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            // Initialize with 500 if unexpected as default
            var code = HttpStatusCode.InternalServerError; 
            switch (ex)
            {
                case NotFoundException _:
                    code = HttpStatusCode.NotFound;
                    break;
                case NoContentException _:
                    code = HttpStatusCode.NoContent;
                    break;
                case BadRequestException _:
                    code = HttpStatusCode.BadRequest;
                    break;
            }

            var result = JsonConvert.SerializeObject(new { error = ex.Message });
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)code;
            return context.Response.WriteAsync(result);
        }
    }
}
