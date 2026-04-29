using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
    [Route("api/unitree-groups")]
    [ApiController]
    public class UniTreeGroupsController : ControllerBase
    {
        private readonly UniTreeGroupServices _service;

        public UniTreeGroupsController(UniTreeGroupServices service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateGroupDto dto)
            => Ok(await _service.CreateGroup(dto));

        [HttpGet]
        public async Task<IActionResult> List()
            => Ok(await _service.GetAllGroups());

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
            => Ok(await _service.GetGroupById(id));

        [HttpPost("{id}/join")]
        public async Task<IActionResult> Join(int id, [FromBody] int userId)
        {
            await _service.JoinGroupAsync(new JoinGroupDto { UniTreeGroupId = id, UserId = userId });
            return Ok(new { Message = "Successfully joined the group." });
        }

        [HttpGet("{id}/members")]
        public async Task<IActionResult> GetMembers(int id)
            =>  Ok(await _service.GetMembers(id));
    }
}