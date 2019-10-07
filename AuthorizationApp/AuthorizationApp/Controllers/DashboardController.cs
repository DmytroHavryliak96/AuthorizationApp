using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AuthorizationApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace AuthorizationApp.Controllers
{

    [Authorize (Policy = "ApiUser")]
    [Route("api/[controller]/[action]")]
    public class DashboardController : Controller
    {
        private readonly ClaimsPrincipal caller;
        private readonly ApplicationContext db;

        public DashboardController(ApplicationContext db, IHttpContextAccessor httpContext)
        {
            this.db = db;
            this.caller = httpContext.HttpContext.User;
            
        }

        [HttpGet]
        public async Task<IActionResult> Home()
        {
            var userId = caller.Claims.Single(c => c.Type == "id");
            var customer = await db.Customers.Include(c => c.Identity).SingleAsync(c => c.Identity.Id == userId.Value);

            return new OkObjectResult(new
            {
                Message = "This is secure API and user data!",
                customer.Identity.FirstName,
                customer.Identity.LastName,
                customer.Identity.GoogleEmail,
                customer.Location
            });
        }
    }
}