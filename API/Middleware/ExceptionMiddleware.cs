using System;
using System.Net;
using System.Text.Json;
using API.Errors;
using Azure.Core;

namespace API.Middleware;

public class ExceptionMiddleware(IHostEnvironment env,RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex,env);
        }
        
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception ex,IHostEnvironment env)
    {
         context.Response.ContentType = "application/json";
         context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
         var Response=env.IsDevelopment()? new ApiErrorResponse(context.Response.StatusCode, ex.Message, ex.StackTrace?.ToString())
         : new ApiErrorResponse(context.Response.StatusCode,ex.Message, "Internal Server Error");
         var options=new JsonSerializerOptions{PropertyNamingPolicy=System.Text.Json.JsonNamingPolicy.CamelCase};
         var json=JsonSerializer.Serialize(Response,options);
         return  context.Response.WriteAsync(json);

    }
}
