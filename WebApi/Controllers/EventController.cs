using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApi.Models;

namespace WebApi.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class EventController : ControllerBase
    {
        private readonly AccountContext _context;

        public EventController(AccountContext context)
        {
            _context = context;
        }

        // GET: Event
        [HttpGet]
        public IEnumerable<Account> GetAccount()
        {
            return _context.Account;
        }

        // GET: Event/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetAccount([FromRoute] string id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var account = await _context.Account.FindAsync(id);

            if (account == null)
            {
                return NotFound();
            }

            return Ok(account);
        }

        // PUT: Event/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAccount([FromRoute] string id, [FromBody] Account account)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != account.Id)
            {
                return BadRequest();
            }

            _context.Entry(account).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AccountExists(id))
                {
                    return NotFound(0);
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }


        // POST: Event
        [HttpPost]
        public async Task<dynamic> PostAccountBalance([FromBody] Event accountBalance)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                dynamic retorno = null;                

                if (accountBalance.type == "withdraw")
                { 
                    Account accObj = new Account();

                    if (!AccountExists(accountBalance.origin))
                        return NotFound(0);

                    var accountExist = (OkObjectResult)GetAccount(accountBalance.origin).Result;
                    accObj = accountExist.Value as Account;

                    accObj.Balance -= accountBalance.amount;

                    await PutAccount(accObj.Id, accObj);

                    return CreatedAtAction(nameof(GetAccount), new { Origin = accObj });

                }
                else if (accountBalance.type == "deposit")
                {          

                    if (!AccountExists(accountBalance.destination))
                    {
                        Account account = new Account();
                        account.Id = accountBalance.destination;
                        account.Balance = accountBalance.amount;

                        return CreatedAtAction(nameof(GetAccount), new Transfer { Destination = await CreateAccount(account) });
                    }
                    else
                    {
                        Account accObj = new Account();

                        var accountExist = (OkObjectResult)GetAccount(accountBalance.destination).Result;
                        accObj = accountExist.Value as Account;

                        accObj.Balance += accountBalance.amount;

                        await PutAccount(accObj.Id, accObj);
                        
                        return CreatedAtAction(nameof(GetAccount), new Transfer { Destination = accObj });
                    }              
                }
                else if (accountBalance.type == "transfer")
                {                  
                    //withdraw
                    Account originCount = new Account();

                    if (!AccountExists(accountBalance.origin))
                        return NotFound(0);

                    var accountExist = (OkObjectResult)GetAccount(accountBalance.origin).Result;
                    originCount = accountExist.Value as Account;

                    originCount.Balance -= accountBalance.amount;

                    await PutAccount(originCount.Id, originCount);                    

                    //deposit
                    Account destinationCount = new Account();

                    if (!AccountExists(accountBalance.destination))
                    {
                        destinationCount.Id = accountBalance.destination;
                        destinationCount.Balance = accountBalance.amount;

                        _context.Account.Add(destinationCount);
                        await _context.SaveChangesAsync();
                        CreatedAtAction(nameof(GetAccount), new { id = destinationCount.Id }, destinationCount);
                    }
                    else
                    {
                        accountExist = (OkObjectResult)GetAccount(accountBalance.destination).Result;
                        destinationCount = accountExist.Value as Account;

                        destinationCount.Balance += accountBalance.amount;
                    }                    

                    await PutAccount(destinationCount.Id, destinationCount);

                    retorno = CreatedAtAction(nameof(GetAccount), new { Origin = originCount, Destination = destinationCount });
                }

                return retorno;
            }
            catch (Exception e)
            {
                throw e;
            }
            
        }

        // DELETE: Event/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAccount([FromRoute] long id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var account = await _context.Account.FindAsync(id);
            if (account == null)
            {
                return NotFound(0);
            }

            _context.Account.Remove(account);
            await _context.SaveChangesAsync();

            return Ok(account);
        }

        private bool AccountExists(string id)
        {
            return _context.Account.Any(e => e.Id == id);
        }

        private async Task<Account> CreateAccount(Account account)
        {
            _context.Account.Add(account);
            await _context.SaveChangesAsync();
            CreatedAtAction(nameof(GetAccount), new { id = account.Id }, account);
            return account;
        }

    }
}