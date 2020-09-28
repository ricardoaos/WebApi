using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApi.Models;

namespace WebApi.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ResetController : ControllerBase
    {
        private readonly AccountContext _context;

        public ResetController(AccountContext context)
        {
            _context = context;
        }

        // POST: api/Reset
        [HttpPost]
        public IActionResult PostAccount()
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Database.EnsureDeleted();

            return Ok();
        }

    }
}