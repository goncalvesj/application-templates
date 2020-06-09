using FluentValidation;
using NetCore.Durable.Functions.Dto;

namespace NetCore.Durable.Functions.Validators
{
	public class EventDtoValidator : AbstractValidator<EventDto>
	{
		public EventDtoValidator()
		{
			RuleFor(x => x.Event).NotEmpty();
        }
	}
}