using HuntleyWeb.Application.Data.Repos;
using HuntleyWeb.Application.Services;
using HuntleyWeb.Application.Services.Email;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Net.Mail;

namespace HuntleyWeb.Application.Configuration
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApplication(this IServiceCollection services, MailOptions mailOptions)
        {
            var smtpClient = new SmtpClient(mailOptions.Server, mailOptions.PortNumber)
            {
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(mailOptions.MailUser, mailOptions.ServerPassword),
                EnableSsl = mailOptions.EnableSSL
            };

            services.AddTransient<IMailMessageService>((svc) =>
            {
                var logger = svc.GetService<ILogger<MailMessageService>>();
                
                return new MailMessageService(smtpClient, logger);
            });

            return services;
        }

        public static IServiceCollection AddBookingRateRepository(this IServiceCollection services, CosmosOptions configuration)
        {
            services
                .AddOptions<CosmosOptions>()
                .Configure(options =>
                {
                    options.ConnectionString = configuration.ConnectionString;
                    options.DatabaseName = configuration.DatabaseName;
                    options.ContainerName = configuration.ContainerName;
                    options.TtlSecs = configuration.TtlSecs;
                });

            services.AddScoped<CosmosClient>(c =>
            {
                return new CosmosClient(configuration.ConnectionString);
            });

            services.AddScoped<IBookingRateRepository, BookingRateRepository>();

            return services;
        }
    }
}
