using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BankTransactionsManagement.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LogicTestController : ControllerBase
    {
        [Authorize]
        [HttpGet("test")]
        public async Task<string> TestLogic()
        {
            // Your logic here
            string result = "Hello, this is a test of the logic in the LogicTestController!";
            return await Task.FromResult(result.ToUpper());
        }
    }
}