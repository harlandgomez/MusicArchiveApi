using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MusicArchiveApi.Adapters;
using MusicArchiveApi.Configuration;
using MusicArchiveApi.Interfaces;
using MusicArchiveApi.Middleware;
using MusicArchiveApi.Services;

namespace MusicArchiveApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        /// <summary>
        /// This method gets called by the runtime. Use this method to add services to the container.
        /// </summary>
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            services.Configure<BaseUrlSettings>(Configuration.GetSection("BaseUrl"));
            services.AddScoped<IMusicBrainAdapter, MusicBrainzAdapter>();
            services.AddScoped<IWikidataAdapter, WikidataAdapter>();
            services.AddScoped<IWikipediaAdapter, WikipediaAdapter>();
            services.AddScoped<ICoverArtAdapter, CoverArtAdapter>();
            services.AddScoped<IMusicBrainzService, MusicBrainzService>();
        }

        /// <summary>
        /// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// </summary>
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseMiddleware(typeof(ErrorHandler));
            app.UseMvc();
        }
    }
}
