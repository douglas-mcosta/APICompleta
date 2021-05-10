// using Microsoft.AspNetCore.Builder;
// using Microsoft.Extensions.DependencyInjection;
// using Elmah.Io.AspNetCore;
// using System;
// using Elmah.Io.Extensions.Logging;
// using Microsoft.Extensions.Logging;
// using Microsoft.Extensions.Configuration;
// using DevIO.Api.Extensions;
// using Microsoft.AspNetCore.Diagnostics.HealthChecks;
// using HealthChecks.UI.Client;
// using Elmah.Io.AspNetCore.HealthChecks;

// namespace DevIO.Api.Configuration
// {
//     public static class LoggerConfig
//     {
//         public static IServiceCollection AddLoggingConfiguration(this IServiceCollection services, IConfiguration configuration)
//         {
//             services.AddElmahIo(o =>
//              {
//                  o.ApiKey = "e8225e2fe0584d88828da7b75b202b22";
//                  o.LogId = new Guid("0438a7b0-cbb5-449d-b6bf-6b2f48824b46");
//              });
             
//             services.AddHealthChecks()
//             .AddElmahIoPublisher(options =>
//             {
//                 options.ApiKey = "e8225e2fe0584d88828da7b75b202b22";
//                 options.LogId = new Guid("0438a7b0-cbb5-449d-b6bf-6b2f48824b46");
//                 options.HeartbeatId = "316c9018e17f45d08fc14d124286ce8a";
//                 options.Application = "Minha API Completa";
//             })
//             .AddCheck("Produtos", new SqlServerHealthCheck(configuration.GetConnectionString("DefaultConnection")))
//             .AddSqlServer(configuration.GetConnectionString("DefaultConnection"));

//             services.AddHealthChecksUI()
//                 .AddInMemoryStorage();
//             // services.AddLogging(builder =>
//             // {
//             //     builder.AddElmahIo(o =>
//             //    {
//             //        o.ApiKey = "e8225e2fe0584d88828da7b75b202b22";
//             //        o.LogId = new Guid("0438a7b0-cbb5-449d-b6bf-6b2f48824b46");
//             //    });
//             //     builder.AddFilter<ElmahIoLoggerProvider>("null", LogLevel.Warning);
//             // });
//             return services;
//         }
//         public static IApplicationBuilder UseLoggingConfiguration(this IApplicationBuilder app)
//         {
//             app.UseElmahIo();
//             app.UseHealthChecks("/api/hc", new HealthCheckOptions()
//             {
//                 Predicate = _ => true,
//                 ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
//             });

//             app.UseHealthChecksUI(options =>
//             {
//                 options.UIPath = "/api/hc-ui";
//                 options.ResourcesPath = "/api/hc-ui-resources";

//                 options.UseRelativeApiPath = false;
//                 options.UseRelativeResourcesPath = false;
//                 options.UseRelativeWebhookPath = false;
//             });
//             return app;
//         }
//     }
// }