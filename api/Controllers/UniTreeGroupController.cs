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
        public IActionResult Create([FromBody] CreateGroupDto dto)
            => Ok(_service.CreateGroup(dto));

        [HttpGet]
        public IActionResult List()
            => Ok(_service.GetAllGroups());

        [HttpGet("{id}")]
        public IActionResult Get(int id)
            => Ok(_service.GetGroupById(id));

        [HttpPost("{id}/join")]
        public async Task<IActionResult> Join(int id, [FromBody] int userId)
        {
            await _service.JoinGroupAsync(new JoinGroupDto { UniTreeGroupId = id, UserId = userId });
            return Ok(new { Message = "Successfully joined the group." });
        }

        [HttpGet("{id}/members")]
        public IActionResult GetMembers(int id)
            => Ok(_service.GetMembers(id));
    }
}