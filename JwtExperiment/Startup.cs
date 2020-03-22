using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Threading.Tasks;

namespace JwtExperiment
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddAuthentication("OAuth")
                .AddJwtBearer("OAuth", config => {
                    config.Events = new JwtBearerEvents()
                    {
                        OnMessageReceived = ctx =>
                        {
                            if (ctx.Request.Query.ContainsKey("access_token"))
                                ctx.Token = ctx.Request.Query["access_token"];

                            return Task.CompletedTask;
                        }
                    };

                    config.TokenValidationParameters = new TokenValidationParameters()
                    {
                        ValidIssuer = Constants.Issuer,
                        ValidAudience = Constants.Audience,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Constants.SecretKey))                        
                    };
                });

            services.AddControllersWithViews();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
            });
        }
    }
}
