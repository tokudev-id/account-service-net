namespace AccountService.Server.Middleware
{
    public class SecurityHeadersMiddleware
    {
        private readonly RequestDelegate _next;

        public SecurityHeadersMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var headers = context.Response.Headers;

            headers["Content-Security-Policy"] = "default-src 'self'; script-src 'self'; object-src 'none'";
            headers["X-Frame-Options"] = "SAMEORIGIN";
            headers["X-Content-Type-Options"] = "nosniff";
            headers["Referrer-Policy"] = "strict-origin-when-cross-origin";
            headers["Permissions-Policy"] = "geolocation=(), microphone=(), camera=()";

            await _next(context);
        }
    }
}
