namespace MicroBluer.TimeTask
{
    using System;
    using Hangfire;
    using Hangfire.SQLite;
    using Hangfire.Server;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.DependencyInjection;


    public static class AspNetCoreExtensions
    {
        /// <summary>
        /// 任务域服务（单机模式）
        /// </summary>
        public static IApplicationBuilder UseTimeTask(this IApplicationBuilder app)
        {
            app.UseHangfireDashboard();
            app.UseHangfireServer();
            return app;
        }

        public static void AddTimeTask(this IServiceCollection services, string dataConnectString)
        {
          //  services.Add<ITaskProvider>(new HangfireTaskProvider());
            services.AddHangfire(x => x.UseSQLiteStorage(dataConnectString));
        }

    }
}
