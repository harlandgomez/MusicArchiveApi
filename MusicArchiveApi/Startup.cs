using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MusicArchiveApi.Adapters;
using MusicArchiveApi.Configuration;
using MusicArchiveApi.Interfaces;

namespace MusicArchiveApi
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
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            services.Configure<BaseUrlSettings>(Configuration.GetSection("BaseUrl"));
            services.AddScoped<IMusicBrainAdapter, MusicBrainzAdapter>();
            services.AddScoped<IWikidataAdapter, WikidataAdapter>();
            services.AddScoped<IWikipediaAdapter, WikipediaAdapter>();
            services.AddScoped<ICoverArtAdapter, CoverArtAdapter>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();
        }
    }
}
