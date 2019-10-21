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
        public ViewModelToEntityMappingProfile()
        {
            CreateMap<RegistrationViewModel, AppUser>()
                .ForMember(ap => ap.UserName, map => map.MapFrom(vm => vm.Email));
        }
    }

    public class AssociateModelToEntityMappingProfile : Profile
    {
        public AssociateModelToEntityMappingProfile()
        {
            CreateMap<AssociateViewModel, AppUser>()
                .ForMember(ap => ap.UserName, map => map.MapFrom(am => am.OriginalEmail))
                .ForMember(ap => ap.Email, map => map.MapFrom(am => am.OriginalEmail));
        }
    }
}
