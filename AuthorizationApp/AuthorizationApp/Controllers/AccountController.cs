using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AuthorizationApp.Helpers;
using AuthorizationApp.Models;
using AuthorizationApp.ViewModels;
using AutoMapper;
using Microsoft.AspNetCore.Identity;

namespace AuthorizationApp.Controllers
{
    [Route("api/[controller]")]
    public class AccountController : Controller
    {
        private readonly ApplicationContext db;
        private readonly UserManager<AppUser> manager;
        private readonly IMapper mapper;

        public AccountController(ApplicationContext db, UserManager<AppUser> manager, IMapper mapper)
        {
            this.db = db;
            this.manager = manager;
            this.mapper = mapper;
        }
        
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] RegistrationViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userIdentity = mapper.Map<AppUser>(model);

            var result = await manager.CreateAsync(userIdentity, model.Password);

            if (!result.Succeeded) return new BadRequestObjectResult(Errors.AddErrorsToModelState(result, ModelState));

            await db.Customers.AddAsync(new Customer { IdentityId = userIdentity.Id, Location = model.Location });
            await db.SaveChangesAsync();
            
            return new OkObjectResult("Account created");
        }
    }
}
