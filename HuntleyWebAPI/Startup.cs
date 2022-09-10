using HuntleyWeb.Application.Services;
using HuntleyWeb.Application.Services.Email;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System.Net;
using System.Net.Mail;
using System.Reflection;
using MediatR;
using HuntleyWeb.Application.Commands.Email;

namespace HuntleyWebAPI
{
    public class Startup
    {
        private AssemblyName _assemblyName = Assembly.GetEntryAssembly().GetName();
        private string _version;
        private string  _name;
        private string _title;

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;

            _version = "1.1";
            _title = "HuntleyWebAPI";
            _name = _assemblyName.Name;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddApplicationInsightsTelemetry();

            var host = Configuration.GetValue<string>("MailServer");
            var port = Configuration.GetValue<int>("MailServerPort");
            var mailUserName = Configuration.GetValue<string>("MailServerUser");
            var mailUserPwd = Configuration.GetValue<string>("MailServerPwd");
            var enableSSL = Configuration.GetValue<bool>("EnableSSL");

            var smtpClient = new SmtpClient(host, port)
            {
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(mailUserName, mailUserPwd),
                EnableSsl = enableSSL
            };                  

            services.AddTransient<IMailMessageService>((svc) =>
            {
                var logger = svc.GetService<ILogger<MailMessageService>>();
                var config = svc.GetService<IConfiguration>();

                return new MailMessageService(smtpClient, logger);
            });

            services.AddMediatR(typeof(MailMessageCommand));

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc($"v{_version}", new OpenApiInfo { Title = _title, Description = "HuntleyWeb API Services", Version = $"v{_version}" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            var swaggerEndpoint = $"/swagger/v{_version}/swagger.json";

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSwagger();

            //app.UseSwaggerUI(c =>
            //{
            //    c.SwaggerEndpoint(swaggerEndpoint, $"v1");
            //    c.DocumentTitle = $"{_name} v{_version}";
            //    c.DocExpansion(DocExpansion.None);
            //}); 

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint(swaggerEndpoint, _name);
            });

            app.UseExceptionHandler("/error");

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
