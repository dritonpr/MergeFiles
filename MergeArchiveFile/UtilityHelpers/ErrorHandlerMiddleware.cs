using System.Globalization;
using System.Net;
using System.Text.Json;

namespace MergeArchiveFile.UtilityHelpers
{
    public class ErrorHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger _logger;

        public ErrorHandlerMiddleware(RequestDelegate next, ILogger<ErrorHandlerMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }
        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception error)
            {
                var response = context.Response;
                response.ContentType = "application/json";

                switch (error)
                {
                    case AppException e:
                        response.StatusCode = (int)HttpStatusCode.BadRequest;
                        break;
                    case KeyNotFoundException e:
                        response.StatusCode = (int)HttpStatusCode.NotFound;
                        break;
                    default:
                        response.StatusCode = (int)HttpStatusCode.InternalServerError;
                        break;
                }
                _logger.LogError($"Something went wrong! Message: {error?.Message}");

                var result = new ErrorDetails
                {
                    StatusCode = context.Response.StatusCode,
                    Message = "This should not have happened. We are already working on a solution!"
                }.ToString();
                await response.WriteAsync(result);
            }
        }


        public class AppException : Exception
        {
            public AppException() : base() { }

            public AppException(string message) : base(message) { }

            public AppException(string message, params object[] args)
                : base(String.Format(CultureInfo.CurrentCulture, message, args))
            {
            }
        }
        public class ErrorDetails
        {
            public int StatusCode { get; set; }
            public string Message { get; set; }
            public override string ToString()
            {
                return JsonSerializer.Serialize(this);
            }
        }
    }
}
