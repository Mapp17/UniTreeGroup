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
        public IActionResult Schedule([FromBody] CreatePayoutDto dto)
        {
            var result = _payoutServices.SchedulePayout(dto);
            return Ok(result);
        }

        [HttpGet("group/{id}")]
        public IActionResult GetGroup(int id)
        {
            var results = _payoutServices.GetPayoutGroup(id);
            return Ok(results);
        }
    }
}