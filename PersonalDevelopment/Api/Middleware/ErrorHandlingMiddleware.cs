namespace PersonalDevelopment.Api.Middleware
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ErrorHandlingMiddleware> _logger;

        public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
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
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception for {Path}", context.Request.Path);

                context.Response.StatusCode = 500;
                context.Response.ContentType = "application/json";
                var problem = new
                {
                    title = "An unexpected error occurred.",
                    status = 500,
                    detail = ex.Message,
                    traceId = context.TraceIdentifier
                };
                await context.Response.WriteAsJsonAsync(problem);
            }
        }
    }

}
