using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ACNH_Marketplace.DataBase;
using ACNH_Marketplace.DataBase.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ACNH_Marketplace.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        private MarketplaceContext _context;

        public TestController(MarketplaceContext context)
        {
            _context = context;
        }

        [HttpGet, Route("getUser")]
        public async Task<ActionResult<User>> GetUser([FromQuery] int id)
        {
            return await _context.Users.FindAsync(id);
        }
    }
}