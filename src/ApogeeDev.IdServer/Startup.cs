using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using ApogeeDev.IdServer.ApplicationServices;
using ApogeeDev.IdServer.ApplicationServices.Abstractions;
using ApogeeDev.IdServer.Core.Config;
using ApogeeDev.IdServer.Core.Config.Default;
using ApogeeDev.IdServer.Core.EntityModels;
using ApogeeDev.IdServer.Repositories;
using IdentityServer4;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;

namespace ApogeeDev.IdServer
{
    public class Startup
    {
        public Startup(IHostingEnvironment environment, IConfiguration configuration)
        {
            Configuration = configuration;
            Environment = environment;
            MigrationsAssembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;
        }

        public IHostingEnvironment Environment { get; }
        public IConfiguration Configuration { get; }

        public string MigrationsAssembly { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddOptions();
            services.Configure<IdentityServerOptions>(Configuration.GetSection(nameof(IdentityServerOptions)));
            services.Configure<GoogleConfig>(Configuration.GetSection(nameof(GoogleConfig)));

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            ConfigureDependencyInjection(services);

            ConfigureAspNetIdentityService(services);

            ConfigureIdentityServer(services);
        }

        private void ConfigureAspNetIdentityService(IServiceCollection services)
        {
            services.AddDbContext<ApplicationDbContext>(ConfigureDbContextAction(MigrationsAssembly));

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();
        }

        private void ConfigureDependencyInjection(IServiceCollection services)
        {
            services.AddTransient<IUserService, UserService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            InitializeDatabase(app);

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            //app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseIdentityServer();
            app.UseMvcWithDefaultRoute();
        }

        private void ConfigureIdentityServer(IServiceCollection services)
        {
            var builder = services.AddIdentityServer(options =>
            {
                options.Events.RaiseErrorEvents = true;
                options.Events.RaiseInformationEvents = true;
                options.Events.RaiseFailureEvents = true;
                options.Events.RaiseSuccessEvents = true;
            })
            .AddAspNetIdentity<ApplicationUser>();

            if (Environment.IsDevelopment())
            {
                builder.AddDeveloperSigningCredential();
            }
            else
            {
                throw new Exception("need to configure key material");
            }

            // configure identity server with in-memory stores, keys, clients and scopes
            builder
                //.AddTestUsers(Config.GetUsers())
                // this adds the config data from DB (clients, resources)
                .AddConfigurationStore(options =>
                {
                    options.ConfigureDbContext = ConfigureDbContextAction(MigrationsAssembly);
                })
                // this adds the operational data from DB (codes, tokens, consents)
                .AddOperationalStore(options =>
                {
                    options.ConfigureDbContext = ConfigureDbContextAction(MigrationsAssembly);

                    // this enables automatic token cleanup. this is optional.
                    options.EnableTokenCleanup = true;
                });

            var gcfg = new GoogleConfig();
            Configuration.Bind(nameof(GoogleConfig), gcfg);

            services.AddAuthentication()
                .AddGoogleOpenIdConnect(ConfigureGoogleOAuth);
        }
        private Action<DbContextOptionsBuilder> ConfigureDbContextAction(string migrationsAssembly) =>
            b => b.UseMySql(Configuration.GetConnectionString("Database"),
                mySqlOptions =>
                {
                    mySqlOptions.ServerVersion(new Version(10, 3, 11), ServerType.MySql);
                    mySqlOptions.MigrationsAssembly(migrationsAssembly);
                });

        private void InitializeDatabase(IApplicationBuilder app)
        {
            using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                serviceScope.ServiceProvider.GetRequiredService<PersistedGrantDbContext>().Database.Migrate();

                var context = serviceScope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();
                context.Database.Migrate();
                if (!context.Clients.Any())
                {
                    foreach (var client in ApplicationData.Clients)
                    {
                        context.Clients.Add(client.ToEntity());
                    }
                    context.SaveChanges();
                }

                if (!context.IdentityResources.Any())
                {
                    foreach (var resource in ApplicationData.Identities)
                    {
                        context.IdentityResources.Add(resource.ToEntity());
                    }
                    context.SaveChanges();
                }

                if (!context.ApiResources.Any())
                {
                    foreach (var resource in ApplicationData.Apis)
                    {
                        context.ApiResources.Add(resource.ToEntity());
                    }
                    context.SaveChanges();
                }
            }
        }

        private void ConfigureGoogleOAuth(OpenIdConnectOptions oidcOptions)
        {
            var gcfg = new GoogleConfig();
            Configuration.Bind(nameof(GoogleConfig), gcfg);

            oidcOptions.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;
            oidcOptions.SignOutScheme = IdentityServerConstants.SignoutScheme;

            oidcOptions.ClientId = gcfg.ClientId;
            oidcOptions.ClientSecret = gcfg.ClientSecret;
            oidcOptions.ResponseType = OpenIdConnectResponseType.CodeIdToken;
            oidcOptions.GetClaimsFromUserInfoEndpoint = true;
            oidcOptions.SaveTokens = true;
            //oidcOptions.ClaimActions.Add (new GoogleClaimsProcessor (AppClaimTypes.GoogleImageUrl));
            //oidcOptions.Events = new OpenIdConnectEvents {
            //    OnUserInformationReceived = UserInformationReceived
            //};
            oidcOptions.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = true,
                ValidateIssuer = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true
            };
            /*if(!string.IsNullOrWhiteSpace(AppPathPrefix))
            {
                var callbackPath = $"{AppPathPrefix}{oidcOptions.CallbackPath}";
                logger.LogDebug($"Setting google callback path: {callbackPath}");
                oidcOptions.CallbackPath = callbackPath;
            }*/

            oidcOptions.Events.OnRedirectToIdentityProviderForSignOut = (ctx) =>
            {
                var c = ctx;
                return Task.CompletedTask;
            };
        }
    }
}