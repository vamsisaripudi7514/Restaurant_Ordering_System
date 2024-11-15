using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ROS.Implement.Repository;

namespace ROS.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private IAuthRepository _authRepository;

        public AuthController(IAuthRepository authRepository)
        {
            _authRepository = authRepository;
        }

        [HttpGet("Login")]
        public IActionResult Login()
        {
            try
            {
                string _strMsg = _authRepository.Login();
                return Ok(_strMsg);
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }

        }
    }
}
