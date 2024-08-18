using FluentValidation;
using HuntleyServicesAPI.Models;

namespace HuntleyServicesAPI.Controllers.Validators
{
    public class MessageValidator : AbstractValidator<ContactMessage>
    {
        public MessageValidator() 
        {
            RuleFor((ContactMessage message) => message.TargetAddress).NotEmpty()
                .WithMessage("The  field {PropertyName} is required.");
        }
    }
}
