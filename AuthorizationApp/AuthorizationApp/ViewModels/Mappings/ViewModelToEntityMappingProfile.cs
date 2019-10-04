using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuthorizationApp.Models;
using AutoMapper;

namespace AuthorizationApp.ViewModels.Mappings
{
    public class ViewModelToEntityMappingProfile : Profile
    {
        ViewModelToEntityMappingProfile()
        {
            CreateMap<RegistrationViewModel, AppUser>()
                .ForMember(ap => ap.UserName, map => map.MapFrom(vm => vm.Email));
        }
    }
}
