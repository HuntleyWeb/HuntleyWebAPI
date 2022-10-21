using HuntleyWeb.Application.Commands.BookingRates.Command.enums;
using System;

namespace HuntleyWeb.Application.Commands.BookingRates.Command
{
    public class RateCommandResult
    {
        public bool Success { get; set; }

        public Guid RecordId { get; set; }  
        
        public CommandActionResult CommandResult { get; set; }

        public int RecordsAffected { get; set; }

        public string Result 
        { 
            get
            {
                return CommandResult.ToString();
            } 
        }

        public string Information { get; set; }
    }
}
