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
        public async Task<IActionResult> Deposit([FromBody] TransactionRequestDto dto)
            => Ok(await _service.ProcessDeposit(dto));

        [HttpPost("withdraw")]
        public async Task<IActionResult> Withdraw([FromBody] TransactionRequestDto dto)
            => Ok(await _service.ProcessWithdraw(dto));

        [HttpPost("contribute")]
        public async Task<IActionResult> Contribute([FromBody] TransactionRequestDto dto)
            => Ok(await _service.ProcessContribute(dto));

        [HttpGet("user/{id}")]
        public IActionResult GetUserTransactions(int id)
            => Ok(_service.GetUserHistory(id));

        [HttpGet("{id}/ledger")]
        public IActionResult GetLedger(int id)
            => Ok(_service.GetLedgerEntry(id));
    }
}