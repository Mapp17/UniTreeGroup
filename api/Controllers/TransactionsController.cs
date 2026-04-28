using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionsController : ControllerBase
    {
        private readonly TransactionsServices _service;

        public TransactionsController(TransactionsServices service)
        {
            _service = service;
        }

        [HttpPost("deposit")]
        public IActionResult Deposit([FromBody] TransactionRequestDto dto)
            => Ok(_service.ProcessDeposit(dto));

        [HttpPost("withdraw")]
        public IActionResult Withdraw([FromBody] TransactionRequestDto dto)
            => Ok(_service.ProcessWithdraw(dto));

        [HttpPost("contribute")]
        public IActionResult Contribute([FromBody] TransactionRequestDto dto)
            => Ok(_service.ProcessContribute(dto));

        [HttpGet("user/{id}")]
        public IActionResult GetUserTransactions(int id)
            => Ok(_service.GetUserHistory(id));

        [HttpGet("{id}/ledger")]
        public IActionResult GetLedger(int id)
            => Ok(_service.GetLedgerEntry(id));
    }
}