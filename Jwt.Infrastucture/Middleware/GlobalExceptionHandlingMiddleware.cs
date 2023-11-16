using Jwt.Infrastucture.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Jwt.Infrastucture.Middleware
{
    public class GlobalExceptionHandlingMiddleware : IMiddleware
    {
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                await next(context);
            }
            catch (Exception ex)
            {

            }
        }

        public static Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            context.Response.ContentType = "application/json";

            var problemDetails = new ProblemDetails
            {
                Title = ex.Message,
                Type = ex.Source
            };

            switch(ex)
            {
                case UserNotFoundException _:
                    problemDetails.Status = StatusCodes.Status404NotFound;
                    context.Response.StatusCode = StatusCodes.Status404NotFound;
                    break;
                case WrongInputException _:
                    problemDetails.Status = StatusCodes.Status400BadRequest;
                    context.Response.StatusCode= StatusCodes.Status400BadRequest;
                    break;
                default:
                    problemDetails.Status = StatusCodes.Status500InternalServerError;
                    context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                    break;
            }

            var json = JsonSerializer.Serialize(problemDetails);
            return context.Response.WriteAsync(json);
        }
    }
}
