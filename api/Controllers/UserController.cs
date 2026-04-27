using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserServices _userServices;
        public UserController(UserServices userServices)
        {
            _userServices = userServices;
        }

        [HttpPost("register")]
        public IActionResult CreateUser(CreateDto newUser)
        {
            var createdUser = _userServices.CreateNewUser(newUser);
            return Ok(createdUser);
        }

        [HttpGet]
        public IActionResult GetUsers()
        {
            var users = _userServices.GetAllUsers();
            return Ok(users);
        }

        [HttpGet("{id}/wallet")]
        public IActionResult GetUser(int id)
        {
            var user = _userServices.GetUserWallet(id);
            return Ok(user);
        } 

    }
}