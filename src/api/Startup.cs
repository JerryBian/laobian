using System.IO;
using Laobian.Api.HostedServices;
using Laobian.Share.Blog.Repository;
using Laobian.Share.Blog.Service;
using Laobian.Share.Command;
using Laobian.Share.Converter;
using Laobian.Share.Setting;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

namespace Laobian.Api
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
            services.Configure<BlogSetting>(setting => setting.Setup(Configuration));
            services.Configure<ApiSetting>(setting => setting.Setup(Configuration));

            services.AddSingleton<ICommandClient, ProcessCommandClient>();
            services.AddSingleton<IBlogService, BlogService>();
            services.AddSingleton<IBlogReadonlyRepository, BlogPostRepository>();
            services.AddSingleton<IBlogReadWriteRepository, BlogDbRepository>();

            services.AddHostedService<BlogHostedService>();
            var dpFolder = Configuration.GetValue<string>("DATA_PROTECTION_KEY_PATH");
            var sharedCookieName = Configuration.GetValue<string>("SHARED_COOKIE_NAME");
            Directory.CreateDirectory(dpFolder);
            services.AddDataProtection().PersistKeysToFileSystem(new DirectoryInfo(dpFolder)).SetApplicationName("LAOBIAN");
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
                {
                    options.Cookie.Name = sharedCookieName;
                });
            services.AddControllers().AddJsonOptions(config =>
            {
                var converter = new IsoDateTimeConverter();
                config.JsonSerializerOptions.Converters.Add(converter);
            });
            services.AddSwaggerGen(c => { c.SwaggerDoc("v1", new OpenApiInfo {Title = "Api", Version = "v1"}); });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
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