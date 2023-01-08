using HuntleyWeb.Application.Commands.BookingRates.Command;
using HuntleyWeb.Application.Commands.BookingRates.Query;
using HuntleyWeb.Application.Commands.Bookings.Command;
using HuntleyWeb.Application.Commands.Email;
using MediatR;

namespace HuntleyServicesAPI.Configuration
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddFunctionServices(this IServiceCollection services)
        {
            services.AddMediatR(typeof(MailMessageCommand));
            services.AddMediatR(typeof(BookingRateCommand));
            services.AddMediatR(typeof(BookingRateQueryHandler));
            services.AddMediatR(typeof(BookingCommand));

            return services;
        }
    }
}
