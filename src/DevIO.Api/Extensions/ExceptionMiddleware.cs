// using System;
// using System.Net;
// using System.Threading.Tasks;
// using Elmah.Io.AspNetCore;
// using Microsoft.AspNetCore.Http;
// namespace DevIO.Api.Extensions
// {
//     public class ExceptionMiddleware
//     {
//         private readonly RequestDelegate _next;
//         public ExceptionMiddleware(RequestDelegate next)
//         {
//             _next = next;
//         }
//         public async Task InvokeAsync(HttpContext context)
//         {
//             try
//             {
//                 await _next(context);
//             }
//             catch (Exception ex)
//             {
//                 HandleExceptionAsync(context, ex);
//             }
//         }
//         public static void HandleExceptionAsync(HttpContext context, Exception ex)
//         {
//             ex.Ship(context);
//             context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
//         }
//     }
// }