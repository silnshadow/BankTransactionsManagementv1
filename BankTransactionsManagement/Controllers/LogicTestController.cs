using System.Text;
using BankTransactionsManagement.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BankTransactionsManagement.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LogicTestController : ControllerBase
    {
        [HttpGet("test")]
        public async Task<string> TestLogic()
        {
            // Your logic here
            string result = "Hello, this is a test of the logic in the LogicTestController!";
            return await Task.FromResult(result.ToUpper());
        }
        private readonly AppDbContext _context;

        public LogicTestController(AppDbContext context)
        {
            _context = context;
        }

        [Authorize]
        [HttpPost("CreateTransaction")]
        public async Task<IActionResult> CreateTransaction([FromBody] Transaction transaction)
        {
            if (transaction == null)
                return BadRequest("Transaction data is required.");
            _context.Transactions.Add(transaction);
            int createdEntryId = await _context.SaveChangesAsync();

            return Ok(createdEntryId);
        }

    [HttpPost("withdraw")]
    [Authorize(Policy = "CanWithdraw")]
    public IActionResult Withdraw(/* params */)
    {
        // Withdraw logic
        return Ok();
    }

    [HttpGet("check")]
    [Authorize(Policy = "CanCheck")]
    public IActionResult Check(/* params */)
    {
        // Check logic
        return Ok();
    }
    }
}