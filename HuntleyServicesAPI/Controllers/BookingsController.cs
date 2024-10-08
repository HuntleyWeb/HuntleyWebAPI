﻿using HuntleyWeb.Application.Commands.BookingRates.Query;
using HuntleyWeb.Application.Commands.Bookings.Command;
using HuntleyWeb.Application.Commands.Bookings.Query;
using HuntleyWeb.Application.Commands.Email;
using HuntleyWeb.Application.Commands.enums;
using HuntleyWeb.Application.Data.Models.Bookings;
using HuntleyWeb.Application.Data.Models.Requests;
using HuntleyWeb.Application.Models;
using HuntleyWeb.Application.Services.Bookings;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Swashbuckle.AspNetCore.Annotations;
using System.Net;

namespace HuntleyServicesAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BookingsController : ControllerBase
    {
        private const string DateFormat = "dd/MM/yyyy";

        private readonly IMediator _mediator;
        private readonly IConfiguration _configuration;
        private readonly ILogger<BookingsController> _logger;

        public BookingsController(IMediator mediator, IConfiguration configuration, ILogger<BookingsController> logger)
        {
            _mediator = mediator;
            _configuration = configuration;
            _logger = logger;
        }

        [HttpPost]
        [Route("createBooking")]
        [SwaggerResponse((int)HttpStatusCode.Created, Description = "Successfully Created New Booking")]
        [SwaggerResponse((int)HttpStatusCode.BadRequest)]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> AddNewBookingRate([FromBody] BookingRequest bookingRequest)
        {
            var (validationMessage, isValid) = BookingsHelper.BookingsDatesAreValid(bookingRequest.StartDate, bookingRequest.EndDate);
            
            if (!isValid)
            {
                return BadRequest(validationMessage);
            }

            var weekNumber = BookingsHelper.GetBookingWeekNumber(bookingRequest.StartDate);

            var booking = new Booking
            {
                Name = bookingRequest.Name,
                EmailAddrress= bookingRequest.EmailAddrress,
                Mobile = bookingRequest.Mobile,
                Cottage= bookingRequest.Cottage,
                StartDate = bookingRequest.StartDate,
                EndDate = bookingRequest.EndDate,                
                WeekNumber = weekNumber,
                Created = DateTime.Now
            };
                     
            // Get Booking Rate
            var rateResult = await _mediator.Send(
                new BookingRateQuery
                {
                    RequestedYear = booking.StartDate.Year,
                    RequestedWeekNumber = booking.WeekNumber
                });           

            var command = new BookingCommand
            {
                Booking = booking,
                Rate = rateResult?.Rate,
                RequestedAction = CommandAction.Insert
            };

            var result = await _mediator.Send(command);

            if (result.Success) {

                // Send Emails
                var notificationCommand = new MailMessageCommand
                {
                    MessageRequest = new MailMessageRequest
                    {
                        FromAddress = "HuntleyWeb@hotmail.com",
                        TargetAddress = _configuration.GetValue<string>("Bookings:TargetEmail"),
                        Subject = "Booking Request",
                        MessageBody = BookingsHelper.GetNotificationContent(booking),
                        NotificationList = "",
                        BodyIsHtml = true
                    }
                };

                var notificationResult = await _mediator.Send(notificationCommand);
            }

            var response = new OkObjectResult(result)
            {
                StatusCode = result.CommandResult switch
                {
                    CommandActionResult.Created => (int)HttpStatusCode.Created,
                    CommandActionResult.Conflict => (int)HttpStatusCode.Conflict,
                    _ => (int)HttpStatusCode.InternalServerError
                }
            };

            return response;
        }

        [HttpGet]
        [Route("getBookingDates")]
        [SwaggerResponse((int)HttpStatusCode.BadRequest)]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetBookingDates(string month, string year)
        {
            if (!int.TryParse(month, out var bookingMonth))
                return BadRequest("Invalid Month Number");

            if (!int.TryParse(year, out var bookingYear))
                return BadRequest("Invalid Year");

            if (bookingMonth < 1 || bookingMonth > 12)
                return BadRequest("WeekNumber must be bewteen 1 and 12");

            var startDate = new DateTime(bookingYear, bookingMonth, 1);
            var endDate = startDate.AddMonths(13);

            var query = new BookingQuery
            {
                StartDate = startDate,
                EndDate = endDate,
            };

            var result = await _mediator.Send(query);

            var bookedDays = new string[] { };
            if (result.Bookings.Any())
            {
                bookedDays = BookingsHelper.GetBookedDays(result.Bookings);
            }           

            return Ok(bookedDays);           
        }
    }
}
