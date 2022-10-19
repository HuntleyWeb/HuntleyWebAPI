using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using System.Xml.Linq;
using System;
using HuntleyWeb.Application.Commands.Email;
using HuntleyWeb.Application.Services.Email;
using HuntleyWeb.Application.Services;
using Microsoft.OpenApi.Models;
using System.Net.Mail;
using System.Net;

namespace HuntleyServicesAPI
{
    public class Startup
    {
        //private AssemblyName _assemblyName = Assembly.GetEntryAssembly().GetName();
        private string _version;
        //private string _name;
        private string _title;

        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;

            _version = "1.1";
            _title = "HuntleyWebAPI";
            //_name = _assemblyName.Name;
        }

        public void Configure(WebApplication app, IWebHostEnvironment env)
        {
            if (!app.Environment.IsDevelopment())
            {
                //app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                //app.UseHsts();

                app.UseSwagger();
                app.UseSwaggerUI();
            }
            app.UseHttpsRedirection();
            //app.UseStaticFiles();
            //app.UseRouting();
            app.UseAuthorization();
          
            app.MapControllers();
            app.Run();
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            //services.AddApplicationInsightsTelemetry();
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();

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

            //services.AddMediatR(typeof(MailMessageCommand));

            //services.AddSwaggerGen(c =>
            //{
            //    c.SwaggerDoc($"v{_version}", new OpenApiInfo { Title = _title, Description = "HuntleyWeb API Services", Version = $"v{_version}" });
            //});
        }
    }
}
