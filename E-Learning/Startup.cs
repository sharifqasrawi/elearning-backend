using System;
using System.Threading.Tasks;
using E_Learning.Models;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using E_Learning.Helpers;
using E_Learning.Repositories;
using Microsoft.Extensions.FileProviders;
using System.IO;
using Microsoft.AspNetCore.Http;
using E_Learning.Hubs;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace E_Learning
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
            services.AddCors();
            services.AddDbContextPool<ApplicationDBContext>(
               options => options.UseSqlServer(Configuration.GetConnectionString("DB"))
               );
            services.AddAutoMapper(typeof(Startup));

            services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.Password.RequiredLength = 5;
                options.Password.RequiredUniqueChars = 1;
                options.SignIn.RequireConfirmedEmail = true;

            }).AddEntityFrameworkStores<ApplicationDBContext>()
              .AddDefaultTokenProviders();


            // configure strongly typed settings objects
            var appSettingsSection = Configuration.GetSection("AppSettings");
            services.Configure<AppSettings>(appSettingsSection);

            // configure jwt authentication
            var appSettings = appSettingsSection.Get<AppSettings>();
            var key = Encoding.ASCII.GetBytes(appSettings.Secret);
            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(x =>
            {
                x.Events = new JwtBearerEvents
                {
                    OnTokenValidated = context =>
                    {
                        var dbcontext = context.HttpContext.RequestServices.GetRequiredService<ApplicationDBContext>();
                        var userId = context.Principal.Identity.Name;

                        var user = dbcontext.Users.Find(userId);
                        if (user == null)
                        {
                            // return unauthorized if user no longer exists
                            context.Fail("Unauthorized");
                        }
                        return Task.CompletedTask;
                    }
                };
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };
            });

            services.AddScoped<ICountryRepository, SqlCountryRepository>();
            services.AddScoped<IAboutRepository, SqlAboutRepository>();
            services.AddScoped<ICategoryRepository, SqlCategoryRepository>();
            services.AddScoped<IMessageRepository, SqlMessageRepository>();
            services.AddScoped<IEmailMessageRepository, SqlEmailMessageRepository>();
            services.AddScoped<IDirectoryRepository, SqlDirectoryRepository>();
            services.AddScoped<IUploadedFileRepository, SqlUploadedFileRepository>();
            services.AddScoped<ICourseRepository, SqlCourseRepository>();
            services.AddScoped<ICourseRatingRepository, SqlCourseRatingRepository>();
            services.AddScoped<ISectionRepository, SqlSectionRepository>();
            services.AddScoped<ISessionRepository, SqlSessionRepository>();
            services.AddScoped<ISessionContentRepository, SqlSessionContentRepository>();
            services.AddScoped<IDoneSessionRepository, SqlDoneSessionsRepository>();
            services.AddScoped<ITagRepository, SqlTagRepository>();
            services.AddScoped<ILikeRepository, SqlLikeRepository>();
            services.AddScoped<ICommentRepository, SqlCommentRepository>();
            services.AddScoped<INotificationRepository, SqlNotificationRepository>();
            services.AddScoped<IClassRepository, SqlClassRepository>();
            services.AddScoped<IFavoriteRepository, SqlFavoriteRepository>();
            services.AddScoped<ISavedSessionRepository, SqlSavedSessionsRepository>();
            services.AddScoped<IAppRatingRepository, SqlAppRatingRepository>();
            services.AddScoped<IReportRepository, SqlReportRepository>();
            services.AddScoped<IQuizRepository, SqlQuizRepository>();
            services.AddScoped<IUserQuizRepository, SqlUserQuizRepository>();
            services.AddScoped<IVisitRepository, SqlVisitRepository>();

            services.AddSingleton<ITranslator, Translator>();

            services.AddHttpContextAccessor();
            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();



            //services.AddMvc(options =>
            //{
            //    options.Filters.Add(new CustomAuthorizeFilter(new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build()));
            //});

            services.AddControllers().AddNewtonsoftJson(options =>
                options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
            );

            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy", builder => builder
                //.WithOrigins("http://localhost:4200")
                .WithOrigins("https://*.qasrawi.fr/*")
                .SetIsOriginAllowedToAllowWildcardSubdomains()
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials());
            });

            services.AddSignalR();

            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "ClientApp/dist";
            });

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseCors("CorsPolicy");

            app.UseStaticFiles();

            app.UseDirectoryBrowser(new DirectoryBrowserOptions()
            {
                FileProvider = new PhysicalFileProvider(Path.Combine(System.IO.Directory.GetCurrentDirectory(), @"wwwroot", "appData", "Uploads")),
                RequestPath = new PathString("/appData/uploads")
            });

            app.UseRouting();



            app.UseAuthentication();
            app.UseAuthorization();

            if (!env.IsDevelopment())
            {
                app.UseSpaStaticFiles();
            }

            //app.UseForwardedHeaders(new ForwardedHeadersOptions
            //{
            //    ForwardedHeaders = ForwardedHeaders.XForwardedFor |
            //    ForwardedHeaders.XForwardedProto
            //});

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<SignalHub>("/signalHub", options =>
                {
                    options.Transports =
                        HttpTransportType.WebSockets |
                        HttpTransportType.LongPolling;
                });
            });

           

            app.UseSpa(spa =>
            {
                // To learn more about options for serving an Angular SPA from ASP.NET Core,
                // see https://go.microsoft.com/fwlink/?linkid=864501

                spa.Options.SourcePath = "ClientApp";

               
            });
        }
    }
}
