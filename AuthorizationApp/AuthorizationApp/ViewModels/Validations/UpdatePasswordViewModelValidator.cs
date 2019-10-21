using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;


namespace AuthorizationApp.ViewModels.Validations
{
    public class UpdatePasswordViewModelValidator : AbstractValidator<UpdatePasswordViewModel>
    {
        public UpdatePasswordViewModelValidator()
        {
            RuleFor(vm => vm.Password).NotEmpty().WithMessage("Password cannot be empty");
        }
    }
}
