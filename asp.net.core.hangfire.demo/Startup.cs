using Hangfire;
using Hangfire.MemoryStorage;
using Hangfire.MySql;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace asp.net.core.hangfire.demo
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRazorPages();
            //使用内存
            //services.AddHangfire(config =>
            //                    config.SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
            //                        .UseSimpleAssemblyNameTypeSerializer()
            //                        .UseDefaultTypeSerializer()
            //                        .UseMemoryStorage());
            //使用mysql
            services.AddHangfire(config =>
                              config.SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                                  .UseSimpleAssemblyNameTypeSerializer()
                                  .UseRecommendedSerializerSettings()
                                  .UseStorage(new MySqlStorage("server=127.0.0.1;user id = root;password=123456;database=hangfire;pooling=true;charset=utf8;Allow User Variables=True;",new MySqlStorageOptions
                                  {
                                      //CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
                                      //SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
                                      //QueuePollInterval = TimeSpan.Zero,
                                      //UseRecommendedIsolationLevel = true,
                                      //DisableGlobalLocks = true
                                  })));


            services.AddHangfireServer();

            services.AddSingleton<IPrintJob, PrintJob>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, 
            IWebHostEnvironment env, 
            IBackgroundJobClient backgroundJobClient,
            IRecurringJobManager recurringJobManager,
            IServiceProvider serviceProvider
            )
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }

            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseHangfireDashboard();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
            });
            backgroundJobClient.Enqueue(() => Console.WriteLine("Hello world"));

            recurringJobManager.AddOrUpdate(
                    "Run every minute",
                    () => serviceProvider.GetService<IPrintJob>().Print(),
                    //cron表达式
                    "* * * * *"
                    );
        }
    }
}
