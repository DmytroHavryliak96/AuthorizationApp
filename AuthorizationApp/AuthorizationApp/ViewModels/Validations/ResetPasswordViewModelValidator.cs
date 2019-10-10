using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;

namespace AuthorizationApp.ViewModels.Validations
{
    public class ResetPasswordViewModelValidator : AbstractValidator<ResetPasswordViewModel>
    {
        public ResetPasswordViewModelValidator()
        {
            RuleFor(rm => rm.Email).NotEmpty().WithMessage("Email cannot be empty");
            RuleFor(rm => rm.Email).EmailAddress();
            RuleFor(rm => rm.Code).NotEmpty().WithMessage("Code cannot be empty");
            RuleFor(rm => rm.Password).NotEmpty().WithMessage("New password cannot be empty");
        }
    }
}
