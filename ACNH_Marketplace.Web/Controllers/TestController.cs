using ACNH_Marketplace.DataBase;
using ACNH_Marketplace.DataBase.Models;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace ACNH_Marketplace.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        private readonly MarketplaceContext _context;

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