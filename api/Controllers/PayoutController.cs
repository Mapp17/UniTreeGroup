using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PayoutsController : ControllerBase
    {
        private readonly PayoutServices _payoutServices;

        public PayoutsController(PayoutServices payoutServices)
        {
            _payoutServices = payoutServices;
        }

        [HttpPost("schedule")]
        public async Task<IActionResult> Schedule([FromBody] CreatePayoutDto dto)
        {
            var result = await _payoutServices.SchedulePayoutAsync(dto);
            return Ok(result);
        }

        [HttpGet("group/{id}")]
        public async Task<IActionResult> GetGroup(int id)
        {
            var results = await _payoutServices.GetPayoutGroupAsync(id);
            return Ok(results);
        }
    }
}