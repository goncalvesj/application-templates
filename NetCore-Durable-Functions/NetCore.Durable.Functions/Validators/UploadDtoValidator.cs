using FluentValidation;
using NetCore.Durable.Functions.Dto;

namespace NetCore.Durable.Functions.Validators
{
	public class UploadDtoValidator : AbstractValidator<UploadDto>
	{
		public UploadDtoValidator()
		{
			RuleFor(x => x.ProjectName).NotEmpty();
			RuleFor(x => x.FileName).NotEmpty();
		}
	}
}