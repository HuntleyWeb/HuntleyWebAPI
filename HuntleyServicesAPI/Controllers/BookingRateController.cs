using Azure.Core;
using HuntleyServicesAPI.Models;
using HuntleyWeb.Application.Commands.BookingRates.Command;
using HuntleyWeb.Application.Commands.BookingRates.Query;
using HuntleyWeb.Application.Commands.enums;
using HuntleyWeb.Application.Data.Models.Bookings;
using HuntleyWeb.Application.Data.Models.Requests;
using HuntleyWeb.Application.Services.Bookings;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Net;

namespace HuntleyServicesAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BookingRateController : ControllerBase
    {

        private readonly IMediator _mediator;
        private readonly IConfiguration _configuration;
        private readonly ILogger<BookingRateController> _logger;

        public BookingRateController(IMediator mediator, IConfiguration configuration, ILogger<BookingRateController> logger)
        {
            _mediator = mediator;
            _configuration = configuration;
            _logger = logger;
        }

        [HttpPost]
        [Route("createBookingRate")]
        [SwaggerResponse((int)HttpStatusCode.Created, Description = "Successfully Created New Rate")]
        [SwaggerResponse((int)HttpStatusCode.BadRequest)]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> AddNewBookingRate([FromBody] CalendarRate rate)
        {
            var minYear = DateTime.Now.Year - 1;

            if (!int.TryParse(rate?.Year, out var year))            
                return BadRequest("Invalid Year");

            if (year < minYear)
                return BadRequest($"Year must be on or after :{minYear}");

            if (!int.TryParse(rate?.WeekNumber, out var weekNumber))
                return BadRequest("Invalid WeekNumber");

            if (weekNumber < 1 || weekNumber > 52)
                return BadRequest("WeekNumber must be bewteen 1 and 52");

            var bookingRate = new BookingRate
            {
                Year = year,
                WeekNumber = weekNumber,
                WeekendRate = rate.WeekendRate,
                MidWeekRate = rate.MidWeekRate,
                SevenDayRate = rate.SevenDayRate,
                AvailableForRental = rate.Available
            };

            var command = new BookingRateCommand
            {
                Rate = bookingRate,
                RequestedAction = CommandAction.Insert
            };

            var result = await _mediator.Send(command);

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

        [HttpPut]
        [Route("updateBookingRate")]
        [SwaggerResponse((int)HttpStatusCode.NoContent, Description = "Successfully Updated Booking Rate")]
        [SwaggerResponse((int)HttpStatusCode.BadRequest)]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> UpdateBookingRate([FromBody] BookingRate rate)
        {
            var minYear = DateTime.Now.Year - 1;            

            var command = new BookingRateCommand
            {
                Rate = rate,
                RequestedAction = CommandAction.Update
            };

            var result = await _mediator.Send(command);

            var response = new OkObjectResult(result)
            {
                StatusCode = result.CommandResult switch
                {
                    CommandActionResult.NotFound => (int)HttpStatusCode.NotFound,                   
                    CommandActionResult.Updated => (int)HttpStatusCode.NoContent,                    
                    _ => (int)HttpStatusCode.InternalServerError
                }                
            };

            return response;
        }

        [HttpDelete]
        [Route("deleteBookingRate")]
        [SwaggerResponse((int)HttpStatusCode.NoContent, Description = "Successfully Deleted Booking Rate")]
        [SwaggerResponse((int)HttpStatusCode.BadRequest)]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> DeleteBookingRate(string documentId, string year)
        {
            if (!Guid.TryParse(documentId, out var bookingRateId))
                return BadRequest("Invalid Booking Rate Id");

            if (!int.TryParse(year, out var bookingYear))
                return BadRequest("Invalid Year");            

            var command = new BookingRateCommand
            {
                Rate = new BookingRate
                {
                    Id = bookingRateId,
                    Year = bookingYear
                },               
                RequestedAction = CommandAction.Delete
            };

            var result = await _mediator.Send(command);

            var response = new OkObjectResult(result)
            {
                StatusCode = result.CommandResult switch
                {
                    CommandActionResult.Deleted => (int)HttpStatusCode.NoContent,
                    CommandActionResult.NotFound => (int)HttpStatusCode.NotFound,
                    _ => (int)HttpStatusCode.InternalServerError
                }
            };

            return response;
        }

        [HttpGet]
        [Route("getBookingRate")]
        [SwaggerResponse((int)HttpStatusCode.BadRequest)]
        [SwaggerResponse((int)HttpStatusCode.NotFound)]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetBookingRate(string weekNumber, string year)
        {
            if (!int.TryParse(weekNumber, out var bookingWeek))
                return BadRequest("Invalid Week Number");

            if (!int.TryParse(year, out var bookingYear))
                return BadRequest("Invalid Year");

            if (bookingWeek < 1 || bookingWeek > 52)
                return BadRequest("WeekNumber must be bewteen 1 and 52");

            var query = new BookingRateQuery
            {
                RequestedYear = bookingYear,
                RequestedWeekNumber = bookingWeek
            };

            var result = await _mediator.Send(query);

            if (result.RecordFound)
            {
                return Ok(result.Rate);
            }
            else
            {
                return NotFound();
            }
        }

        [HttpGet]
        [Route("getBookingRateByStartDate")]        
        [SwaggerResponse((int)HttpStatusCode.BadRequest)]
        [SwaggerResponse((int)HttpStatusCode.NotFound)]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetBookingRateByStartDate(string date, int duration)
        {
            if (!DateTime.TryParse(date, out var startDate))
                return BadRequest("Invalid Date");

            var weekNumber = BookingsHelper.GetBookingWeekNumber(startDate);
            var breakType = BookingsHelper.GetBookingType(startDate, duration);

            var query = new BookingRateQuery
            {
                RequestedYear = startDate.Year,
                RequestedWeekNumber = weekNumber
            };

            var result = await _mediator.Send(query);

            if (result.RecordFound)
            {
                var rateAmount = breakType switch
                {
                    BreakType.SevenDay => result.Rate.SevenDayRate,
                    BreakType.Weekend => result.Rate.WeekendRate,
                    BreakType.MidWeek => result.Rate.MidWeekRate,
                    _ => throw new Exception("No matching Booking Rate Set!")
                };

                return Ok(rateAmount);
            }
            else
            {
                return NotFound();
            }
        }
    }
}
