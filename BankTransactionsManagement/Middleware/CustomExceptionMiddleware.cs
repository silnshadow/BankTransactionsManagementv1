using Microsoft.AspNetCore.Http;
using System.IO;
using System.Threading.Tasks;

namespace BankTransactionsManagement.Middleware
{
    public class CustomExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly string _logFilePath;

        public CustomExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
            var logDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Logs");
            if (!Directory.Exists(logDirectory))
                Directory.CreateDirectory(logDirectory);

            _logFilePath = Path.Combine(logDirectory, "requests.log");
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var log = $"{System.DateTime.Now}: {context.Request.Method} {context.Request.Path}{System.Environment.NewLine}";
            using (var streamWriter = new StreamWriter(_logFilePath, true))
            {
                streamWriter.WriteLine(log);
            }
            await _next(context);
        }
    }
}