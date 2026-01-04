using System.Diagnostics;

using System.Text;

namespace Games.Microservice.API.Middlewares
{
    
    public class LoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<LoggingMiddleware> _logger;
        private const int MaxBodyLength = 4096; 

        public LoggingMiddleware(RequestDelegate next, ILogger<LoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            var stopwatch = Stopwatch.StartNew();

            var requestBody = await ReadRequestBodyAsync(context);

            var originalResponseBody = context.Response.Body;
            await using var newResponseBody = new MemoryStream();
            context.Response.Body = newResponseBody;

            try
            {
                await _next(context);
                stopwatch.Stop();

                var responseBody = await ReadResponseBodyAsync(newResponseBody);
                newResponseBody.Seek(0, SeekOrigin.Begin);
                await newResponseBody.CopyToAsync(originalResponseBody);

                _logger.LogInformation(
                    "HTTP {Method} {Path} responded {StatusCode} in {Elapsed}ms\nRequest Body: {RequestBody}\nResponse Body: {ResponseBody}",
                    context.Request.Method,
                    context.Request.Path,
                    context.Response.StatusCode,
                    stopwatch.ElapsedMilliseconds,
                    TruncateIfNecessary(requestBody),
                    TruncateIfNecessary(responseBody)
                );
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                _logger.LogError(ex,
                    "Unhandled exception in HTTP {Method} {Path} after {Elapsed}ms",
                    context.Request.Method,
                    context.Request.Path,
                    stopwatch.ElapsedMilliseconds
                );
                throw;
            }
            finally
            {
                context.Response.Body = originalResponseBody;
            }
        }

        private async Task<string> ReadRequestBodyAsync(HttpContext context)
        {
            var request = context.Request;
            request.EnableBuffering();

            request.Body.Seek(0, SeekOrigin.Begin);
            using var reader = new StreamReader(request.Body, Encoding.UTF8, leaveOpen: true);
            var body = await reader.ReadToEndAsync();
            request.Body.Seek(0, SeekOrigin.Begin);

            return string.IsNullOrWhiteSpace(body) ? "[empty]" : body;
        }

        private async Task<string> ReadResponseBodyAsync(Stream responseBodyStream)
        {
            responseBodyStream.Seek(0, SeekOrigin.Begin);
            using var reader = new StreamReader(responseBodyStream, Encoding.UTF8, leaveOpen: true);
            var body = await reader.ReadToEndAsync();
            return string.IsNullOrWhiteSpace(body) ? "[empty]" : body;
        }

        private static string TruncateIfNecessary(string input)
        {
            return input.Length > MaxBodyLength
                ? input.Substring(0, MaxBodyLength) + "... [truncated]"
                : input;
        }
    }
}
