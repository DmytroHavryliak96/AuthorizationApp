using AuthorizationApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthorizationApp.Services.Interfaces
{
    public interface IJwtTokenService
    {
        public Task<string> GetJwtToken(AppUser identity);
    }
}
