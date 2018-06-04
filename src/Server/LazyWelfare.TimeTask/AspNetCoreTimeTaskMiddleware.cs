namespace LazyWelfare.TimeTask
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;
    using System.Collections.Generic;
    using System.Text;

    class AspNetCoreTimeTaskMiddleware
    {
        private readonly RequestDelegate _next;

        public AspNetCoreTimeTaskMiddleware(RequestDelegate next)
        {
            _next = next ?? throw new ArgumentNullException(nameof(next));

        }

        public Task Invoke(HttpContext httpContext)
        {

            return _next.Invoke(httpContext);
        }
    }
}
