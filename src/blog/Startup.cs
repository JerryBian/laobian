using System;
using System.IO;
using System.Threading;
using Laobian.Blog.Cache;
using Laobian.Share.Blog.Repository;
using Laobian.Share.Command;
using Laobian.Share.Converter;
using Laobian.Share.HttpService;
using Laobian.Share.Log;
using Laobian.Share.Setting;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Laobian.Blog
{
    public class Startup
    {
        private readonly IWebHostEnvironment _env;

        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            _env = env;
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<BlogSetting>(setting => setting.Setup(Configuration));

            services.AddSingleton<ICacheClient, MemoryCacheClient>();
            services.AddSingleton<ICommandClient, ProcessCommandClient>();
            services.AddSingleton<IBlogReadonlyRepository, BlogPostRepository>();
            services.AddSingleton<IBlogReadWriteRepository, BlogDbRepository>();

            services.AddHttpClient<ApiHttpService>();

            if (_env.IsDevelopment())
            {
                // give API service few seconds
                Thread.Sleep(TimeSpan.FromSeconds(10));
            }

            services.AddLogging(config =>
            {
                config.SetMinimumLevel(LogLevel.Debug);
                config.AddDebug();
                config.AddConsole();
                config.AddSystemdConsole();
                config.AddGitFile(c =>
                {
                    var logDir = Path.Combine(Configuration.GetValue<string>("GITHUB_READ_WRITE_REPO_LOCAL_DIR"),
                        Configuration.GetValue<string>("LOG_DIR_NAME"));
                    c.LoggerDir = logDir;
                    c.LoggerName = "Laobian Blog";
                    c.MinLevel = _env.IsProduction() ? LogLevel.Warning : LogLevel.Information;
                });
            });

            var dpFolder = Configuration.GetValue<string>("DATA_PROTECTION_KEY_PATH");
            var sharedCookieName = Configuration.GetValue<string>("SHARED_COOKIE_NAME");
            Directory.CreateDirectory(dpFolder);
            services.AddDataProtection().PersistKeysToFileSystem(new DirectoryInfo(dpFolder))
                .SetApplicationName("LAOBIAN");
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
                {
                    options.Cookie.Name = sharedCookieName;
                    options.Cookie.Domain = _env.IsDevelopment() ? "localhost" : ".laobian.me";
                });
            services.AddControllersWithViews().AddJsonOptions(config =>
            {
                var converter = new IsoDateTimeConverter();
                config.JsonSerializerOptions.Converters.Add(converter);
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();
            else
                app.UseExceptionHandler("/Home/Error");
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    "default",
                    "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}