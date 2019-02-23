using Bazinga.AspNetCore.Authentication.Basic;
using Daifuku.Exceptions;
using Daifuku.Extensions;
using Library.Configurations;
using Library.Exceptions;
using Library.Playlists;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Sushi2;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<GzipCompressionProviderOptions>(options => options.Level = System.IO.Compression.CompressionLevel.Fastest);
            services.AddResponseCompression();

            services.Configure<Kumiko>(options => Configuration.Bind(options));

            services.AddSingleton<IPlaylist, MusicPlaylist>();

            var username = Configuration.GetValue<string>("Username");
            var password = Configuration.GetValue<string>("Password");

            if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password))
            {
                services.AddMvc(config => config.Filters.Add(typeof(ApplicationExceptionFilter<AppException>)))
                .AddJsonOptions(options =>
                {
                    options.SerializerSettings.Formatting = Formatting.None;
                    options.SerializerSettings.TypeNameHandling = TypeNameHandling.None;
                    options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                });

                services.AddAuthentication(BasicAuthenticationDefaults.AuthenticationScheme)
                .AddBasicAuthentication(credentials => Task.FromResult(credentials.username == username && credentials.password == password));
            }
            else
            {
                services.AddMvc(config =>
                {
                    config.Filters.Add(typeof(ApplicationExceptionFilter<AppException>));
                    config.Filters.Add(new AllowAnonymousFilter());
                })
                .AddJsonOptions(options =>
                {
                    options.SerializerSettings.Formatting = Formatting.None;
                    options.SerializerSettings.TypeNameHandling = TypeNameHandling.None;
                    options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                });
            }
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            var provider = new FileExtensionContentTypeProvider();
            provider.Mappings[".webmanifest"] = "application/manifest+json";

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseStaticFiles(new StaticFileOptions
                {
                    ContentTypeProvider = provider
                });
            }
            else
            {
                app.UseExceptionHandler("/error");
                app.UseStatusCodePagesWithReExecute("/error/{0}");

                app.UseStaticFiles(new StaticFileOptions
                {
                    OnPrepareResponse = ctx => ctx.Context.Response.Headers.Append("Cache-Control", "public, max-age=31536000"),
                    ContentTypeProvider = provider
                });

                app.UseResponseCompression();
            }

            app.UseDaifuku();
            app.UseHealthz();

            app.UseAuthentication();

            CultureInfo.DefaultThreadCurrentCulture = Cultures.English;
            CultureInfo.DefaultThreadCurrentUICulture = Cultures.English;

            app.UseMvc();
        }
    }
}