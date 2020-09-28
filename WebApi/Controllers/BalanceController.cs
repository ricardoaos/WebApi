using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Threading.Tasks;
using WebApi.Models;

namespace WebApi.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class BalanceController : ControllerBase
    {
        private readonly AccountContext _context;

        public BalanceController(AccountContext context)
        {
            _context = context;
        }

        // GET: Balance/5
        [HttpGet]
        public async Task<dynamic> GetBalance(string account_id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var account = await _context.Account.FindAsync(account_id);

            if (account == null)
            {
                return NotFound(0);
            }

            return Ok(account.Balance);
        }
    }
}