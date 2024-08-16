using AdresApi.Services;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using System;


namespace AdresApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;
            _env= env;
        }

        public IConfiguration Configuration { get; }
        private readonly IWebHostEnvironment _env;

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAntiforgery(options =>
            {
                options.HeaderName = "X-XSRF-TOKEN";
                options.SuppressXFrameOptionsHeader = false;
            });
            //CORS
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                                            builder =>
                                            {
                                                builder.SetIsOriginAllowed(origin => new Uri(origin).Host == "localhost")
                                                .AllowAnyMethod()
                                                .AllowAnyHeader()
                                                .AllowCredentials();
                                            });
            });
            services.AddControllers();
            services.AddControllersWithViews();
            services.AddScoped<IDatab,Datab>();
            //LOGs - Serilog RollingLog
            Log.Logger = new LoggerConfiguration()
               .ReadFrom.Configuration(Configuration)
               .CreateLogger();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env,
            ILoggerFactory loggerFactory, IAntiforgery antiforgery)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            //app.Use(next => context =>
            //{
            //    string path = context.Request.Path.Value;

            //    if (path != null && path.ToLower().Contains("/api"))
            //    {
            //        var tokens = antiforgery.GetAndStoreTokens(context);
            //        context.Response.Cookies.Append("XSRF-TOKEN", tokens.RequestToken,
            //            new CookieOptions()
            //            {
            //                HttpOnly = false
            //            });
            //    }
            //    return next(context);
            //});
            app.UseCors("CorsPolicy");
            loggerFactory.AddSerilog();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        public class CustomExecutionStrategy : ExecutionStrategy
        {
            public CustomExecutionStrategy(DbContext context) : base(context, ExecutionStrategy.DefaultMaxRetryCount, ExecutionStrategy.DefaultMaxDelay)
            {

            }

            protected override bool ShouldRetryOn(Exception exception)
            {
                return exception.GetType() == typeof(DbUpdateException);
            }
        }

    }


}
