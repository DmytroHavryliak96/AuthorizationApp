using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;


namespace AuthorizationApp.ViewModels.Validations
{
    public class ForgotPasswordViewModelValidator : AbstractValidator<ForgotPasswordViewModel>
    {
        public ForgotPasswordViewModelValidator()
        {
            RuleFor(fm => fm.Email).NotEmpty().WithMessage("Email cannot be empty");
            RuleFor(fm => fm.Email).EmailAddress();
        }
    }
}
