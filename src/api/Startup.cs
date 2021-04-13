using System.IO;
using System.Threading.Tasks;
using Laobian.Api.HostedServices;
using Laobian.Share.Blog.Repository;
using Laobian.Share.Blog.Service;
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
using Microsoft.OpenApi.Models;

namespace Laobian.Api
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
            services.Configure<ApiSetting>(setting => setting.Setup(Configuration));

            services.AddSingleton<ICommandClient, ProcessCommandClient>();
            services.AddSingleton<IBlogService, BlogService>();
            services.AddSingleton<IBlogReadonlyRepository, BlogPostRepository>();
            services.AddSingleton<IBlogReadWriteRepository, BlogDbRepository>();

            services.AddHostedService<ApiHostedService>();
            services.AddHttpClient<BlogHttpService>();
            services.AddHttpClient<AdminHttpService>();

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
                    c.LoggerName = "Laobian Api";
                    c.MinLevel = _env.IsProduction() ? LogLevel.Warning : LogLevel.Information;
                });
            });
            var dpFolder = Configuration.GetValue<string>("DATA_PROTECTION_KEY_PATH");
            var sharedCookieName = Configuration.GetValue<string>("SHARED_COOKIE_NAME");
            Directory.CreateDirectory(dpFolder);
            services.AddDataProtection().PersistKeysToFileSystem(new DirectoryInfo(dpFolder))
                .SetApplicationName("LAOBIAN");
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme,
                    options => { options.Cookie.Name = sharedCookieName; });
            services.AddControllers().AddJsonOptions(config =>
            {
                var converter = new IsoDateTimeConverter();
                config.JsonSerializerOptions.Converters.Add(converter);
            });
            services.AddSwaggerGen(c => { c.SwaggerDoc("v1", new OpenApiInfo {Title = "Api", Version = "v1"}); });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostApplicationLifetime appLifetime)
        {
            //var blogHttpService = app.ApplicationServices.GetService<BlogHttpService>();
            //var adminHttpService = app.ApplicationServices.GetService<AdminHttpService>();
            
            //appLifetime.ApplicationStarted.Register(async () =>
            //{
            //    if (blogHttpService != null)
            //    {
            //        await blogHttpService.UpdatePullGitFileEventAsync(true);
            //    }

            //    if (adminHttpService != null)
            //    {
            //        await adminHttpService.UpdatePullGitFileEventAsync(true);
            //    }
            //});

            //appLifetime.ApplicationStopping.Register(async () =>
            //{
            //    if (blogHttpService != null)
            //    {
            //        await blogHttpService.UpdatePullGitFileEventAsync(false);
            //    }

            //    if (adminHttpService != null)
            //    {
            //        await adminHttpService.UpdatePullGitFileEventAsync(false);
            //    }
            //});

            if (_env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Api v1"));
            }

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}